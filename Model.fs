namespace OpilioCraft.StringTemplate

// exceptions
exception InvalidStringTemplateException of Template:string
    with override x.ToString () = $"invalid string template: {x.Template}"

// AST nodes
[<RequireQualifiedAccess>]
type StringTemplateElement =
    | PlainText of Content:string
    | Placeholder of Name:string * Args:string list

type StringTemplate = StringTemplateElement list

type PlaceholderFunc = (string list) -> string
type PlaceholderMap = Map<string, PlaceholderFunc>
