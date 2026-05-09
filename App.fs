module LibraryLab.App

open System
open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open LibraryLab.Logic
open LibraryLab.Html

let private gate = obj ()
let private holds = ResizeArray<HoldNotice> ()

let private writeHtml (ctx: HttpContext) (html: string) =
    ctx.Response.ContentType <- "text/html; charset=utf-8"
    ctx.Response.WriteAsync(html)

let private textOrEmpty (t: string | null) =
    match t with
    | null -> ""
    | x -> x

let private routeText (o: obj | null) =
    match o with
    | null -> ""
    | v -> textOrEmpty (v.ToString())

let private formGet (form: IFormCollection) (name: string) =
    let mutable s = StringValues.Empty

    if form.TryGetValue(name, &s) then
        textOrEmpty (s.ToString())
    else
        ""

let mapRoutes (app: WebApplication) =
    app.MapGet(
        "/",
        RequestDelegate(fun ctx ->
            let q: string =
                match ctx.Request.Query.TryGetValue("q") with
                | true, v when v.Count > 0 ->
                    match v.[0] with
                    | null -> ""
                    | s when s.Length = 0 -> ""
                    | s -> s
                | _ -> ""

            let books = filterCatalog q
            writeHtml ctx (home q books))
    )
    |> ignore

    app.MapGet(
        "/book/{isbn}",
        RequestDelegate(fun ctx ->
            let isbn =
                match ctx.Request.RouteValues.TryGetValue("isbn") with
                | true, v -> routeText v
                | _ -> ""

            match tryFind isbn with
            | Some b -> writeHtml ctx (bookDetail b)
            | None -> writeHtml ctx (notFound ()))
    )
    |> ignore

    app.MapPost(
        "/hold",
        RequestDelegate(fun ctx ->
            task {
                let! form = ctx.Request.ReadFormAsync()
                let reader = formGet form "reader"
                let note = formGet form "note"
                let isbn = formGet form "isbn"

                if String.IsNullOrWhiteSpace(reader) || String.IsNullOrWhiteSpace(isbn) then
                    return! writeHtml ctx (notFound ())
                else
                    let title =
                        match tryFind isbn with
                        | Some b -> b.Title
                        | None -> "Unknown title"

                    let row =
                        {
                            Reader = reader.Trim()
                            Isbn = isbn.Trim()
                            Note = note.Trim()
                            AtUtc = DateTime.UtcNow
                        }

                    lock gate (fun () -> holds.Add(row))
                    return! writeHtml ctx (holdOk reader title)
            })
    )
    |> ignore

    app.MapGet(
        "/holds",
        RequestDelegate(fun ctx ->
            let rows =
                lock gate (fun () -> holds |> Seq.toList)

            writeHtml ctx (holdsPage rows))
    )
    |> ignore
