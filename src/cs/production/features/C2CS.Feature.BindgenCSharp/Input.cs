// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using C2CS.Feature.BindgenCSharp.Data.Model;

namespace C2CS.Feature.BindgenCSharp;

public class Input : UseCaseInput<Output>
{
    public ImmutableArray<string> InputFilePaths { get; }

    public string OutputFilePath { get; }

    public ImmutableArray<CSharpTypeAlias> TypeAliases { get; }

    public ImmutableArray<string> IgnoredNames { get; }

    public string LibraryName { get; }

    public string ClassName { get; }

    public string NamespaceName { get; }

    public string HeaderCodeRegion { get; }

    public string FooterCodeRegion { get; }

    public Input(ConfigurationBindgenCSharp configuration)
    {
        InputFilePaths = VerifyInputFilePaths(configuration.InputFileDirectory);
        OutputFilePath = VerifyOutputFilePath(configuration.OutputFilePath);
        ClassName = VerifyClassName(configuration.ClassName, OutputFilePath);
        LibraryName = VerifyLibraryName(configuration.LibraryName, ClassName);
        NamespaceName = VerifyNamespace(configuration.NamespaceName, LibraryName);
        TypeAliases = VerifyTypeAliases(configuration.MappedTypeNames);
        IgnoredNames = VerifyIgnoredTypeNames(configuration.IgnoredNames);
        HeaderCodeRegion = VerifyHeaderCodeRegion(configuration.HeaderCodeRegionFilePath);
        FooterCodeRegion = VerifyFooterCodeRegion(configuration.FooterCodeRegionFilePath);
    }

    private static ImmutableArray<string> VerifyInputFilePaths(string? inputDirectoryPath)
    {
        string directoryPath;
        if (string.IsNullOrWhiteSpace(inputDirectoryPath))
        {
            directoryPath = Path.Combine(Environment.CurrentDirectory, "ast");
        }
        else
        {
            if (!Directory.Exists(inputDirectoryPath))
            {
                throw new UseCaseException($"The abstract syntax tree input directory '{inputDirectoryPath}' does not exist.");
            }

            directoryPath = inputDirectoryPath;
        }

        var builder = ImmutableArray.CreateBuilder<string>();
        var filePaths = Directory.EnumerateFiles(directoryPath!);
        foreach (var filePath in filePaths)
        {
            var fileName = Path.GetFileName(filePath);
            var runtimePlatformString = fileName.Replace(".json", string.Empty, StringComparison.InvariantCulture);
            var runtimePlatform = RuntimePlatform.FromString(runtimePlatformString);
            if (runtimePlatform == RuntimePlatform.Unknown)
            {
                throw new UseCaseException($"Unknown platform '{runtimePlatform}' for abstract syntax tree.");
            }

            builder.Add(filePath);
        }

        return builder.ToImmutable();
    }

    private static string VerifyOutputFilePath(string? outputFilePath)
    {
        if (!string.IsNullOrEmpty(outputFilePath))
        {
            return Path.GetFullPath(outputFilePath);
        }

        throw new UseCaseException($"The output file path can not be an empty or null string.");
    }

    private static ImmutableArray<CSharpTypeAlias> VerifyTypeAliases(ImmutableArray<CSharpTypeAlias>? mappedTypeNames)
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

    private static ImmutableArray<string> VerifyIgnoredTypeNames(ImmutableArray<string?>? ignoredTypeNames)
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

    private static string VerifyLibraryName(string? libraryName, string className)
    {
        return !string.IsNullOrEmpty(libraryName) ? libraryName : className;
    }

    private static string VerifyNamespace(string? @namespace, string libraryName)
    {
        return !string.IsNullOrEmpty(@namespace) ? @namespace : libraryName;
    }

    private static string VerifyClassName(string? className, string outputFilePath)
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

    private static string VerifyHeaderCodeRegion(string? headerCodeRegionFilePath)
    {
        if (string.IsNullOrEmpty(headerCodeRegionFilePath))
        {
            return string.Empty;
        }

        var code = File.ReadAllText(headerCodeRegionFilePath);
        return code;
    }

    private static string VerifyFooterCodeRegion(string? footerCodeRegionFilePath)
    {
        if (string.IsNullOrEmpty(footerCodeRegionFilePath))
        {
            return string.Empty;
        }

        var code = File.ReadAllText(footerCodeRegionFilePath);
        return code;
    }
}