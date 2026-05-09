module LibraryLab.Html

open System.Net
open LibraryLab.Logic

let esc (s: string) = WebUtility.HtmlEncode(s)

let layoutQ (title: string) (q: string) (inner: string) =
    sprintf
        """<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>%s</title>
  <link rel="stylesheet" href="/css/app.css" />
</head>
<body>
  <header class="bar">
    <a class="logo" href="/">OpenStacks</a>
    <a class="side" href="/holds">Holds</a>
    <form class="search" method="get" action="/" role="search">
      <input type="search" name="q" placeholder="Title, author, tag, ISBN…" value="%s" />
      <button type="submit">Search</button>
    </form>
  </header>
  <main class="shell">%s</main>
  <footer class="fine">Hold requests stay in server memory for this demo session.</footer>
</body>
</html>"""
        (esc title)
        (esc q)
        inner

let home (q: string) (books: Book list) =
    let rows =
        books
        |> List.map (fun b ->
            sprintf
                """<tr>
  <td><a href="/book/%s">%s</a></td>
  <td>%s</td>
  <td><span class="pill">%s</span></td>
  <td>%i</td>
</tr>"""
                (esc b.Isbn)
                (esc b.Title)
                (esc b.Author)
                (esc b.Shelf)
                b.Copies)

        |> String.concat ""

    let intro =
        """<section class="intro"><h1>Find what is on the open shelf</h1>
<p class="sub">Small reading rooms lose track of donations. OpenStacks is a single-room catalogue with a fast filter and a one-step hold note for the volunteer desk.</p></section>"""

    let table =
        sprintf
            """<section class="panel"><h2>Catalogue</h2>
<table class="grid"><thead><tr><th>Title</th><th>Author</th><th>Shelf</th><th>Copies</th></tr></thead><tbody>%s</tbody></table></section>"""
            rows

    layoutQ "Catalogue" q (intro + table)

let bookDetail (b: Book) =
    let body =
        sprintf
            """<article class="panel detail">
  <h1>%s</h1>
  <p class="meta">%s · ISBN %s · %i copies · tags <span class="pill">%s</span></p>
  <p>Need the volunteer to set a copy aside? Leave a short note.</p>
  <form method="post" action="/hold" class="hold">
    <input type="hidden" name="isbn" value="%s" />
    <label>Your name <input name="reader" required maxlength="80" /></label>
    <label>Note <input name="note" maxlength="200" placeholder="Pick up Friday afternoon" /></label>
    <button type="submit">Send hold request</button>
  </form>
  <p><a href="/">← Back</a></p>
</article>"""
            (esc b.Title)
            (esc b.Author)
            (esc b.Isbn)
            b.Copies
            (esc b.Tags)
            (esc b.Isbn)

    layoutQ b.Title "" body

let holdOk (reader: string) (title: string) =
    layoutQ
        "Hold recorded"
        ""
        (sprintf
            """<section class="panel ok"><h1>Request logged</h1>
<p>%s — we will try to tag <strong>%s</strong> for you. Talk to the desk volunteer with your name.</p>
<p><a class="btn" href="/">Return to catalogue</a></p></section>"""
            (esc reader)
            (esc title))

let notFound () =
    layoutQ
        "Missing"
        ""
        """<section class="panel"><h1>We could not find that ISBN</h1><p><a href="/">Search again</a></p></section>"""

let holdsPage (rows: HoldNotice list) =
    let sorted = holdsNewestFirst rows

    let bodyRows =
        if List.isEmpty sorted then
            "<tr><td colspan=\"4\" class=\"muted\">No hold requests yet.</td></tr>"
        else
            sorted
            |> List.map (fun h ->
                sprintf
                    """<tr><td>%s</td><td>%s</td><td>%s</td><td class="muted">%s</td></tr>"""
                    (esc h.Reader)
                    (esc h.Isbn)
                    (esc h.Note)
                    (h.AtUtc.ToString("yyyy-MM-dd HH:mm") + " UTC"))
            |> String.concat ""

    let body =
        sprintf
            """<section class="panel"><h1>Hold requests (this session)</h1>
<p class="sub">Newest first. Data is cleared when the server process restarts.</p>
<table class="grid"><thead><tr><th>Reader</th><th>ISBN</th><th>Note</th><th>When</th></tr></thead><tbody>%s</tbody></table>
<p><a href="/">← Catalogue</a></p></section>"""
            bodyRows

    layoutQ "Holds" "" body
