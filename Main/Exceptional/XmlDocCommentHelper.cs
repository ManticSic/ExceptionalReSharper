/// <copyright file="XmlDocCommentHelper.cs" manufacturer="CodeGears">
///   Copyright (c) CodeGears. All rights reserved.
/// </copyright>

using System;
using System.Xml;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace CodeGears.ReSharper.Exceptional
{
    public class XmlDocCommentHelper
    {
        public static DocumentRange FindRangeOfExceptionTag(XmlNode node, string exceptionType, IDocCommentBlockNode docNode)
        {
            if (docNode == null) return DocumentRange.InvalidRange;

            for (var currentNode = docNode.FirstChild; currentNode != null; currentNode = currentNode.NextSibling)
            {
                var text = currentNode.GetText();
                if (text.Contains("<exception") == false) continue;

                var index = exceptionType.LastIndexOf('.');
                var exceptiontypeName = exceptionType.Substring(index + 1);

                if (text.Contains(exceptiontypeName) && text.Contains(node.InnerText))
                {
                    return currentNode.GetDocumentRange();
                }
            }

            return DocumentRange.InvalidRange;
        }

        public static TextRange InsertExceptionDocumentation(ICSharpTypeMemberDeclarationNode memberDeclaration, string exceptionName)
        {
            var comment = SharedImplUtil.GetDocCommentBlockNode(memberDeclaration);
            var text = comment != null ? comment.GetText() + Environment.NewLine : String.Empty;

            text += String.Format("/// <exception cref=\"{0}\"></exception>", exceptionName) + Environment.NewLine +
                    " public void foo() {}";

            var commentOwner = CSharpElementFactory.GetInstance(memberDeclaration.GetProject()).CreateTypeMemberDeclaration(text) as IDocCommentBlockOwnerNode;
            var docCommentNode = commentOwner.GetDocCommentBlockNode();
            SharedImplUtil.SetDocCommentBlockNode(memberDeclaration, docCommentNode);

            return TextRange.InvalidRange;
        }

        public static void RemoveExceptionDocumentation(ICSharpTypeMemberDeclarationNode memberDeclaration, string documentationText)
        {
            var comment = SharedImplUtil.GetDocCommentBlockNode(memberDeclaration);
            var commentText = comment.GetText();

            var result = commentText.Replace(documentationText, String.Empty);

            result += Environment.NewLine + " public void foo() {}";

            var commentResult = CSharpElementFactory.GetInstance(memberDeclaration.GetProject()).CreateTypeMemberDeclaration(result) as IDocCommentBlockOwnerNode;

            SharedImplUtil.SetDocCommentBlockNode(memberDeclaration, commentResult.GetDocCommentBlockNode());
        }
    }
}