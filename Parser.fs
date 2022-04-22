module OpilioCraft.StringTemplate.Parser

open FParsec

// narrow types
type Parser<'t> = Parser<'t, unit>

// lexical stuff
let pOpeningBrace : Parser<_> = pchar '{'
let pClosingBrace : Parser<_> = pchar '}'
let isNormalChar = function | '{' | '|' | '}' -> false | _ -> true

// elements
let pPlainText : Parser<_> =
    manySatisfy isNormalChar |>> StringTemplateElement.PlainText

let pPlaceholderParts : Parser<_> =
    sepBy1 (many1Satisfy isNormalChar) (pchar '|')

let pPlaceholder : Parser<_> =
    pOpeningBrace >>. pPlaceholderParts .>> pClosingBrace
    |>> fun args -> StringTemplateElement.Placeholder (args.Head, args.Tail)

// string template
let pStringTemplate : Parser<_> =
    manyTill (pPlaceholder <|> pPlainText) eof
