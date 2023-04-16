open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging


type RRule = EWSoftware.PDI.Recurrence

module Db =
  open Npgsql
  open NpgsqlTypes
  
  let connect () =
    task {
      let conn =
        new Npgsql.NpgsqlConnection("Host=localhost; Port=5432; Database=rrules; Username=admin; Password=changeit; Maximum Pool Size = 10")
       
      do! conn.OpenAsync()  
      return conn
    }
  
  let get_rrule () =
    task {
      use! conn = connect()
      use cmd = conn.CreateCommand()
      
      cmd.CommandText <-
        "SELECT rrule from rrules LIMIT 1"
      
      use! reader = cmd.ExecuteReaderAsync()
      
      let result = ResizeArray()
      
      while reader.Read() do
        let rruleStr = reader.GetString(0)
        
        result.Add(EWSoftware.PDI.Recurrence(rruleStr))
        
      return result[0]
     
    }

module Program =

  [<EntryPoint>]
  let main args =

    let builder = WebApplication.CreateBuilder(args)
    let app = builder.Build()
    
    app.MapGet(
      "/instances/{from}/{until}",
      Func<_,_,_>(fun from until ->
        task {
         let! rrule = Db.get_rrule()
         
         return rrule.InstancesBetween(DateTime.Parse from, DateTime.Parse until)
         
        }
        )
      ) |> ignore
   
    app.Run()

    0
