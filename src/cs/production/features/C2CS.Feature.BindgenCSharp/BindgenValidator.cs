// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using System.IO.Abstractions;
using C2CS.Feature.BindgenCSharp.Data;

namespace C2CS.Feature.BindgenCSharp;

public sealed class BindgenValidator : UseCaseValidator<BindgenRequest, BindgenInput>
{
    private readonly IFileSystem _fileSystem;

    public BindgenValidator(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public override BindgenInput Validate(BindgenRequest request)
    {
        var inputFilePaths = InputFilePaths(request.InputFileDirectory);
        var outputFilePath = OutputFilePath(request.OutputFilePath);
        var className = ClassName(request.ClassName, outputFilePath);
        var libraryName = LibraryName(request.LibraryName, className);
        var namespaceName = Namespace(request.NamespaceName, libraryName);
        var typeAliases = TypeAliases(request.MappedTypeNames);
        var ignoredNames = IgnoredTypeNames(request.IgnoredNames);
        var headerCodeRegion = HeaderCodeRegion(request.HeaderCodeRegionFilePath);
        var footerCodeRegion = FooterCodeRegion(request.FooterCodeRegionFilePath);

        return new BindgenInput
        {
            InputFilePaths = inputFilePaths,
            OutputFilePath = outputFilePath,
            ClassName = className,
            LibraryName = libraryName,
            NamespaceName = namespaceName,
            TypeAliases = typeAliases,
            IgnoredNames = ignoredNames,
            HeaderCodeRegion = headerCodeRegion,
            FooterCodeRegion = footerCodeRegion
        };
    }

    private ImmutableArray<string> InputFilePaths(string? inputDirectoryPath)
    {
        string directoryPath;
        if (string.IsNullOrWhiteSpace(inputDirectoryPath))
        {
            directoryPath = _fileSystem.Path.Combine(Environment.CurrentDirectory, "ast");
        }
        else
        {
            if (!_fileSystem.Directory.Exists(inputDirectoryPath))
            {
                throw new UseCaseException($"The abstract syntax tree input directory '{inputDirectoryPath}' does not exist.");
            }

            directoryPath = inputDirectoryPath;
        }

        var builder = ImmutableArray.CreateBuilder<string>();
        var filePaths = _fileSystem.Directory.EnumerateFiles(directoryPath);
        foreach (var filePath in filePaths)
        {
            var fileName = _fileSystem.Path.GetFileName(filePath);
            var platformString = fileName.Replace(".json", string.Empty, StringComparison.InvariantCulture);
            var platform = new TargetPlatform(platformString);
            if (platform == TargetPlatform.Unknown)
            {
                throw new UseCaseException($"Unknown platform '{platform}' for abstract syntax tree.");
            }

            builder.Add(filePath);
        }

        return builder.ToImmutable();
    }

    private string OutputFilePath(string? outputFilePath)
    {
        if (string.IsNullOrEmpty(outputFilePath))
        {
            throw new UseCaseException($"The output file path can not be an empty or null string.");
        }

        var result = _fileSystem.Path.GetFullPath(outputFilePath);
        var directoryPath = _fileSystem.Path.GetDirectoryName(outputFilePath);
        _fileSystem.Directory.CreateDirectory(directoryPath);
        return result;
    }

    private static ImmutableArray<CSharpTypeAlias> TypeAliases(ImmutableArray<CSharpTypeAlias>? mappedTypeNames)
    {
        if (mappedTypeNames == null || mappedTypeNames.Value.IsDefaultOrEmpty)
        {
            return ImmutableArray<CSharpTypeAlias>.Empty;
        }

        var builder = ImmutableArray.CreateBuilder<CSharpTypeAlias>();
        foreach (var typeAlias in mappedTypeNames)
        {
            builder.Add(typeAlias);
        }

        return builder.ToImmutable();
    }

    private static ImmutableArray<string> IgnoredTypeNames(ImmutableArray<string?>? ignoredTypeNames)
    {
        if (ignoredTypeNames == null || ignoredTypeNames.Value.IsDefaultOrEmpty)
        {
            return ImmutableArray<string>.Empty;
        }

        var array = ignoredTypeNames.Value
            .Where(x => !string.IsNullOrEmpty(x))
            .Cast<string>();
        return array.ToImmutableArray();
    }

    private static string LibraryName(string? libraryName, string className)
    {
        return !string.IsNullOrEmpty(libraryName) ? libraryName : className;
    }

    private static string Namespace(string? @namespace, string libraryName)
    {
        return !string.IsNullOrEmpty(@namespace) ? @namespace : libraryName;
    }

    private static string ClassName(string? className, string outputFilePath)
    {
        string result;
        if (string.IsNullOrEmpty(className))
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputFilePath);
            var firstIndexOfPeriod = fileNameWithoutExtension.IndexOf('.', StringComparison.InvariantCulture);
            result = firstIndexOfPeriod == -1
                ? fileNameWithoutExtension
                : fileNameWithoutExtension[..firstIndexOfPeriod];
        }
        else
        {
            result = className;
        }

        return result;
    }

    private static string HeaderCodeRegion(string? headerCodeRegionFilePath)
    {
        if (string.IsNullOrEmpty(headerCodeRegionFilePath))
        {
            return string.Empty;
        }

        var code = File.ReadAllText(headerCodeRegionFilePath);
        return code;
    }

    private static string FooterCodeRegion(string? footerCodeRegionFilePath)
    {
        if (string.IsNullOrEmpty(footerCodeRegionFilePath))
        {
            return string.Empty;
        }

        var code = File.ReadAllText(footerCodeRegionFilePath);
        return code;
    }
}
