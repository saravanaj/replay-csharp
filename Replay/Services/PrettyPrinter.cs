﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Microsoft.CodeAnalysis.Formatting;
using Replay.Model;
using System.Threading.Tasks;

namespace Replay.Services
{
    class PrettyPrinter
    {
        private CSharpObjectFormatter objectFormatter;

        public PrettyPrinter()
        {
            this.objectFormatter = CSharpObjectFormatter.Instance;
        }

        public async Task<FormattedLine> FormatAsync(Document document, EvaluationResult evaluationResult = null)
        {
            var formattedDocument = await Formatter.FormatAsync(document);
            var formattedText = await formattedDocument.GetTextAsync();
            return new FormattedLine(
                formattedText.ToString(),
                FormatObject(evaluationResult?.ScriptResult?.ReturnValue),
                evaluationResult?.Exception?.Message,
                evaluationResult?.StandardOutput
            );
        }

        private string FormatObject(object obj)
        {
            if(obj == null)
            {
                // right now there's no way to determine the difference between "no value" and "null value"
                // intercept all nulls and return null, instead of the string "null"
                // because otherwise every single assignment expression would print "null"
                return null;
            }
            return objectFormatter.FormatObject(obj);
        }
    }
}
