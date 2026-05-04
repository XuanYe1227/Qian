module LibraryLab.Logic

open System

type Book =
    {
        Isbn: string
        Title: string
        Author: string
        Shelf: string
        Tags: string
        Copies: int
    }

let catalog : Book list =
    [
        {
            Isbn = "9781484207413"
            Title = "Expert F# 4.0"
            Author = "Syme, Granicz, Cisternino"
            Shelf = "A-12"
            Tags = "fp;dotnet"
            Copies = 2
        }
        {
            Isbn = "9780137903955"
            Title = "Programming F# 3"
            Author = "Chris Smith"
            Shelf = "A-14"
            Tags = "fp"
            Copies = 1
        }
        {
            Isbn = "9781617297628"
            Title = "Get Programming with F#"
            Author = "Isaac Abraham"
            Shelf = "B-03"
            Tags = "fp;beginner"
            Copies = 3
        }
        {
            Isbn = "9781492053754"
            Title = "Stylish F#"
            Author = "Kit Eason"
            Shelf = "B-03"
            Tags = "style;fp"
            Copies = 1
        }
    ]

let matchesQuery (q: string) (b: Book) =
    if String.IsNullOrWhiteSpace(q) then
        true
    else
        let t = q.Trim().ToLowerInvariant()

        b.Title.ToLowerInvariant().Contains(t)
        || b.Author.ToLowerInvariant().Contains(t)
        || b.Tags.ToLowerInvariant().Contains(t)
        || b.Isbn.Contains(t)

let filterCatalog (q: string) = catalog |> List.filter (matchesQuery q)

let tryFind (isbn: string) =
    let key = isbn.Trim()
    catalog |> List.tryFind (fun b -> b.Isbn.Equals(key, StringComparison.OrdinalIgnoreCase))

type HoldNotice =
    {
        Reader: string
        Isbn: string
        Note: string
        AtUtc: DateTime
    }
