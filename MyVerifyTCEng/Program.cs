using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Collections;

namespace MyVerifyTCEng
{
    class Program
    {

        public static string Retornar_Parametro(string[] args, string Parametro)
        {
            foreach (string aux in args)
                if (aux.Substring(0, aux.IndexOf('=')).ToLower() == Parametro.ToLower())
                    return aux.Substring(aux.IndexOf('=') + 1);

            return "";
        }

        static void Main(string[] args)
        {
            if (args.Length >= 5)
            {
                try
                {
                    string server = Retornar_Parametro(args, "-s");
                    string BD = Retornar_Parametro(args, "-b");
                    string User = Retornar_Parametro(args, "-u");
                    string Pass = Retornar_Parametro(args, "-p");
                    string Valor = Retornar_Parametro(args, "-v");

                    OleDbConnection conn = new OleDbConnection("Provider=sqloledb;Data Source=" + server + ";Initial Catalog=" + BD + ";User Id=" + User + ";Password=" + Pass);
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        OleDbCommand cmd = conn.CreateCommand();

                        //vou buscar todas as tabelas do tceng                    
                        cmd.CommandText = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_CATALOG='"+BD+"'";
                        Queue ListOfTables = new Queue();
                        OleDbDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                            ListOfTables.Enqueue("[" + (string)dr["TABLE_SCHEMA"] + "].[" + (string)dr["TABLE_NAME"] + "]");
                        dr.Close();

                        Console.WriteLine("Aguarde Verificando " + ListOfTables.Count.ToString() + " tabelas ...");
                        //jah peguei a lista de tables do tceng... agora vou procurar o valor em todos os campos das tabelas;
                        foreach (string table in ListOfTables)
                        {
                            Console.WriteLine("Verificando Tabela " + table + " ...");
                            cmd.CommandText = "SELECT * FROM " + table;
                            dr = cmd.ExecuteReader();

                            while (dr.Read())
                            {
                                for (int i = 0; i < dr.FieldCount; i++)
                                    if (dr.GetValue(i).ToString() == Valor)
                                    {
                                        Console.WriteLine("Valor encontrado em : ");
                                        Console.WriteLine("\t Tabela:" + table);
                                        Console.WriteLine("\t Coluna:" + dr.GetName(i).ToLower());
                                    }
                            }
                            dr.Close();
                        }

                    }
                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Sintaxe: MyVerifTcEng -s=<Host Servidor>");
                Console.WriteLine("                      -b=<nome BD>");
                Console.WriteLine("                      -u=<Usuario>");
                Console.WriteLine("                      -p=<Senha>");
                Console.WriteLine("                      -v=<Valor a localizar>");
            }
                            
        }
    }
}
