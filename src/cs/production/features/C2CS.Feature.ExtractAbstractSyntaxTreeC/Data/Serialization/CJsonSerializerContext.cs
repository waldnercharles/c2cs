// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Text.Json;
using System.Text.Json.Serialization;
using C2CS.Feature.ExtractAbstractSyntaxTreeC.Data.Model;

namespace C2CS.Feature.ExtractAbstractSyntaxTreeC.Data.Serialization;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(CAbstractSyntaxTree))]
public partial class CJsonSerializerContext : JsonSerializerContext
{
    public static CJsonSerializerContext Create()
    {
        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        var context = new CJsonSerializerContext(serializerOptions);
        return context;
    }
}