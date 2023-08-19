module OpilioCraft.StringTemplate.Parser

open FParsec

// narrow types
type Parser<'t> = Parser<'t, unit>

// lexical stuff
let pOpeningBrace = pchar '{'
let pClosingBrace = pchar '}'
let isNormalChar = function | '{' | '|' | '}' -> false | _ -> true

// elements
let pPlainText =
    manySatisfy isNormalChar |>> StringTemplateElement.PlainText

let pPlaceholderParts =
    sepBy1 (many1Satisfy isNormalChar) (pchar '|')

let pPlaceholder =
    pOpeningBrace >>. pPlaceholderParts .>> pClosingBrace
    |>> fun args -> StringTemplateElement.Placeholder (args.Head, args.Tail)

// string template
let pStringTemplate : Parser<_> =
    manyTill (pPlaceholder <|> pPlainText) eof
