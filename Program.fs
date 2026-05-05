open Microsoft.AspNetCore.Builder

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    builder.WebHost.UseUrls("http://127.0.0.1:5020") |> ignore
    let app = builder.Build()
    app.UseStaticFiles() |> ignore
    LibraryLab.App.mapRoutes app
    app.Run()
    0
