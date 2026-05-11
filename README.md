# LibraryLab

## Demo

[qian.tryasp.net](http://qian.tryasp.net/)

## About

ASP.NET Core 8 + F#: a small **library-style catalog** (search, book by ISBN, in-memory **hold** queue). No WebSharper. HTML from `Html.fs`, rules in `Logic.fs`, routes in `App.fs`. CSS in `wwwroot/css/`.

## Run (from repo root)

```bash
dotnet build
dotnet run
```

Default local URL: port **5020** (`Properties/launchSettings.json`). If `PORT` is set, `Program.fs` listens on `http://0.0.0.0:{PORT}`.

## Endpoints

- `GET /` - catalog; `?q=` filters titles/tags/authors
- `GET /book/{isbn}` - one book
- `POST /hold` - form: `reader`, `isbn`, optional `note`
- `GET /holds` - holds submitted in this process

## Repo

Ignored: `bin/`, `obj/`, optional `build/`. CI: `.github/workflows/ci.yml`.

## Source files

- `LibraryLab.fsproj` - project
- `Program.fs` - host, static files, `PORT`
- `App.fs` - routes
- `Logic.fs` - catalog + filters
- `Html.fs` - HTML