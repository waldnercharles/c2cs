// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using C2CS.Feature.ExtractAbstractSyntaxTreeC.Data;
using Microsoft.Extensions.DependencyInjection;

namespace C2CS.Feature.ExtractAbstractSyntaxTreeC;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<CJsonSerializer>();
        services.AddTransient<ExtractAbstractSyntaxTreeUseCase>();
    }
}
