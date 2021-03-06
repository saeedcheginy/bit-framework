﻿using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace BitTools.Core.Contracts.HtmlClientProxyGenerator
{
    public interface IDefaultHtmlClientProxyGenerator
    {
        Task GenerateCodes(Workspace workspace);
    }
}