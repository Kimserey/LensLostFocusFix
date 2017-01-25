namespace LensingLostFocus

open WebSharper
open WebSharper.JavaScript
open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client
open WebSharper.JavaScript

[<JavaScript>]
module Lensing =

    type Person = 
        { Name: string
          Aliases: ListModel<int, Alias> }

    and Alias = { Id: int; Value: string }
    
    let aliases = 
        ListModel.Create (fun a -> a.Id) [ { Id = 0; Value = "Bill" }; { Id = 1; Value = "Joe" } ]

    let lensIntoAlias key =
        aliases.LensInto (fun a -> a.Value) (fun a n -> { a with Value = n }) key

    let trigger = Var.Create ()
        
    let Main =
        let content =
            div [

                aliases.View
                |> View.SnapshotOn aliases.Value trigger.View
                |> Doc.BindSeqCached (fun (alias: Alias) ->
                    div
                        [
                            Doc.Input [] (lensIntoAlias alias.Id)
                        ]
                )
                
                hr []

                aliases.View
                |> Doc.BindSeqCached (fun (alias: Alias) ->
                    div 
                        [
                            Doc.TextView (lensIntoAlias alias.Id).View
                        ]
                )
                
                // Needs to be triggered when the list is modified, like add or remove element
                // then we re-render the list of input otherwise it's not needed.
                Doc.Button "Update" [] (fun () -> trigger.Value <- ())
            ]
    
        Doc.RunById "main" content
