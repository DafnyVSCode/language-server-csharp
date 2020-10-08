﻿using DafnyLS.Language;
using DafnyLS.Language.Symbols;
using DafnyLS.Workspace;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DafnyLS.Handlers {
  internal class DafnyHoverHandler : IHoverHandler {
    private readonly ILogger _logger;
    private readonly IDocumentDatabase _documents;

    public DafnyHoverHandler(ILogger<DafnyHoverHandler> logger, IDocumentDatabase documents) {
      _logger = logger;
      _documents = documents;
    }

    public HoverRegistrationOptions GetRegistrationOptions() {
      return new HoverRegistrationOptions {
        DocumentSelector = DocumentSelector.ForLanguage("dafny")
      };
    }

    public Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken) {
      _logger.LogTrace("received hover request for {}", request.TextDocument);
      DafnyDocument? textDocument;
      if(!_documents.TryGetDocument(request.TextDocument, out textDocument)) {
        _logger.LogWarning("the document {} is not loaded", request.TextDocument);
        return Task.FromResult(new Hover());
      }

      ISymbol? symbol;
      if(!textDocument.SymbolLookup.TryGetSymbolAt(request.Position, out symbol)) {
        _logger.LogDebug("no symbol was found at {} in {}", request.Position, request.TextDocument);
        return Task.FromResult(new Hover());
      }
      return Task.FromResult(CreateHover(symbol, cancellationToken));
    }

    private static Hover CreateHover(ISymbol symbol, CancellationToken cancellationToken) {
      return new Hover {
        Contents = new MarkedStringsOrMarkupContent(
          new MarkupContent {
            Kind = MarkupKind.PlainText,
            Value = symbol.GetDetailText(cancellationToken)
          }
        )
      };
    }

    public void SetCapability(HoverCapability capability) {
    }
  }
}