// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Soulgram.Identity.IntegrationEvents;

Console.WriteLine("Hello, World!");



Assembly asm = typeof(DeletedUserEvent).Assembly;

//Assembly asm = Assembly.Load(Assembly.GetEntryAssembly().FullName);
Type type = asm.GetType("Soulgram.Identity.IntegrationEvents.DeletedUserEvent");

Console.WriteLine(type.FullName);
Console.WriteLine(type.ToString());