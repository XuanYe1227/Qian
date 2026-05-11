# LibraryLab

## Demo

[qian.tryasp.net](http://qian.tryasp.net/)

## About

ASP.NET Core 8 + F# library catalog (search, ISBN detail, holds). `Program.fs`: forwarded headers; `PORT` only off IIS.

## Run

```bash
dotnet build
dotnet run
```

Port **5020** locally.

## MonsterASP: `\wwwroot` rule

Host policy: **`\wwwroot` is website root; application files that the web may use must be under this directory.**

Deploy: `dotnet publish -c Release -o publish`, then upload **all** of `publish/` into server `\wwwroot` (`web.config`, `LibraryLab.dll`, other DLLs, JSON, inner `wwwroot/`). Nested `\wwwroot\wwwroot\` is normal.

## Endpoints

- `GET /` - catalog (`?q=`)
- `GET /book/{isbn}` - detail
- `POST /hold` - form
- `GET /holds` - list

## Repo

Ignore `bin/`, `obj/`, `build/`, `_pubcheck/`. CI: `.github/workflows/ci.yml`.

## Files

`LibraryLab.fsproj`, `web.config`, `Program.fs`, `App.fs`, `Logic.fs`, `Html.fs`