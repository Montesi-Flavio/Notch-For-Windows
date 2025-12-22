using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Resources;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Uso: CheckResources <path-to-assembly>");
            return 1;
        }

        var asmPath = args[0];
        if (!File.Exists(asmPath))
        {
            Console.Error.WriteLine("Assembly non trovato: " + asmPath);
            return 2;
        }

        // Carica l'assembly specificato
        var asm = Assembly.LoadFrom(asmPath);
        Console.WriteLine("ManifestResourceNames:");
        foreach (var name in asm.GetManifestResourceNames())
            Console.WriteLine(name);

        using (var s = asm.GetManifestResourceStream("Your.Namespace.MainWindow.xaml"))
            Console.WriteLine(s == null ? "Manca" : "Trovata");

        // Cerca il .g.resources (nome tipico: <DefaultNamespace>.g.resources)
        foreach (var manifestName in asm.GetManifestResourceNames())
        {
            if (manifestName.EndsWith(".g.resources", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine();
                Console.WriteLine("Contenuti di " + manifestName + ":");
                using var stream = asm.GetManifestResourceStream(manifestName);
                using var rr = new ResourceReader(stream);
                foreach (DictionaryEntry entry in rr)
                {
                    Console.WriteLine("  " + entry.Key);
                }

                // Cerca esattamente la voce attesa
                var expected1 = "mainwindow.baml";
                var expected2 = "MainWindow.baml";
                bool found = false;
                foreach (DictionaryEntry entry in rr) { /* no-op: already enumerated above */ }
                // Ri-leggiamo per ricerca semplice
                stream.Position = 0;
                using var rr2 = new ResourceReader(stream);
                foreach (DictionaryEntry entry in rr2)
                {
                    var key = (string)entry.Key;
                    if (key.IndexOf("mainwindow", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Console.WriteLine("Trovata voce correlata: " + key);
                        found = true;
                    }
                }

                if (!found)
                    Console.WriteLine("WARNING: nessuna voce contenente 'mainwindow' trovata in .g.resources");
            }
        }

        return 0;
    }
}