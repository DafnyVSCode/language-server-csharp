﻿using Microsoft.Dafny;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading;

namespace DafnyLS.Language.Symbols {
  internal class VariableSymbol : ISymbol {
    private readonly IVariable _node;

    public string Name => _node.Name;

    public VariableSymbol(IVariable variable) {
      _node = variable;
    }

    public DocumentSymbol AsLspSymbol(CancellationToken cancellationToken) {
      return new DocumentSymbol {
        Name = _node.Name,
        Kind = SymbolKind.Variable,
        Range = _node.Tok.GetLspRange(),
        SelectionRange = GetHoverRange(),
        Detail = GetDetailText(cancellationToken)
      };
    }

    public string GetDetailText(CancellationToken cancellationToken) {
      return $"{_node.Name}:{_node.Type}";
    }

    public Range GetHoverRange() {
      return _node.Tok.GetLspRange();
    }
  }
}
