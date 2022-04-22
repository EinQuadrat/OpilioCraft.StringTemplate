namespace OpilioCraft.StringTemplate

open FParsec

type Runtime () =
    static let mutable parserCache : Map<string, StringTemplate> = Map.empty

    static let parseStringTemplate input : StringTemplate =
        FParsec.CharParsers.run Parser.pStringTemplate input
        |> function
            | Success(stringTemplate, _, _) -> stringTemplate
            | Failure(_, _, _) -> raise <| InvalidStringTemplateException input
        
    static member Parse input =
        parserCache.TryFind input
        |> Option.defaultWith (
            fun _ ->
                let stringTemplate = parseStringTemplate input in // might throw InvalidStringTemplateException
                parserCache <- parserCache |> Map.add input stringTemplate
                stringTemplate
            )

    static member TryParse input = // does not (!) memoize
        try
            input |> parseStringTemplate |> Some
        with
            | _ -> None

    static member Eval (placeholderMap : PlaceholderMap) (onError : string -> string) (stringTemplate : StringTemplate) : string =
        let eval = function
            | StringTemplateElement.PlainText text -> text
            | StringTemplateElement.Placeholder (name, args) ->
                placeholderMap
                |> Map.tryFind name
                |> Option.map (fun placeholderFunc -> placeholderFunc args)
                |> Option.defaultWith (fun _ -> onError name)

        stringTemplate
        |> List.map eval
        |> System.String.Concat

    static member EvalRelaxed (placeholderMap : PlaceholderMap) (stringTemplate : StringTemplate) : string =
        Runtime.Eval placeholderMap (fun _ -> "") stringTemplate
