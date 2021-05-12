using Comun.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace apiexamen
{
    public class ClsExamen
    {
        SqlTransaction transaction;
        SqlConnection connection;
        string url = "https://localhost:5001/api/";

        public static JsonSerializerOptions serializeOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public string ConnectionString { get; set; }

        public ClsExamen(string serverName)
        {
            ConnectionString = @$"Data Source={serverName};Database=BdiExamen;Trusted_Connection=True;";
            connection = new SqlConnection(ConnectionString);
        }

        public List<Examen> ConsultarExamenes()
        {

            List<Examen> examenes = new List<Examen>();

            try
            {
                using (connection)
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("spConsultarTodo", connection);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        var examen = new Examen
                        {
                            IdExamen = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Descripcion = reader.GetString(2)
                        };

                        examenes.Add(examen);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Hubo algun error con la BD: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }


            return examenes;
        }

        public List<Examen> ConsultarExamen(string nombre, string descripcion)
        {

            List<Examen> examenes = new List<Examen>();

            try
            {
                using (connection)
                {
                    using (SqlCommand command = new SqlCommand("spConsultar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = nombre;
                        command.Parameters.Add("@Descripcion", SqlDbType.VarChar).Value = descripcion;

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var examen = new Examen
                            {
                                IdExamen = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2)
                            };

                            examenes.Add(examen);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Hubo algun error con la BD: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }


            return examenes;
        }


        public bool AgregarExamen(Examen examen)
        {
            try
            {
                using (connection)
                {
                    using (SqlCommand command = new SqlCommand("spAgregar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = examen.Nombre;
                        command.Parameters.Add("Descripcion", SqlDbType.VarChar).Value = examen.Descripcion;

                        connection.Open();

                        transaction = connection.BeginTransaction();

                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool ActualizarExamen(Examen examen)
        {
            try
            {
                using (connection)
                {
                    using (SqlCommand command = new SqlCommand("spActualizar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Id", SqlDbType.Int).Value = examen.IdExamen;
                        command.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = examen.Nombre;
                        command.Parameters.Add("Descripcion", SqlDbType.VarChar).Value = examen.Descripcion;

                        connection.Open();

                        transaction = connection.BeginTransaction();
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool EliminarExamen(int Id)
        {

            try
            {
                using (connection)
                {
                    using (SqlCommand command = new SqlCommand("spEliminar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Id", SqlDbType.Int).Value = Id;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                //return false;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<List<Examen>> ConsultarExamenesApi()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(url);
                var response = await client.GetAsync("Examenes");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var examenes = JsonSerializer.Deserialize<List<Examen>>(responseString, serializeOptions);

                    return examenes;
                }
                else
                {
                    return new List<Examen>();
                }
            }
        }

        public async Task<List<Examen>> ConsultarExamenApi(string nombre, string descripcion)
        {

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(url);
                var response = await client.GetAsync($"Examenes/Consultar?nombre={nombre}&descripcion={descripcion}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var examenes = JsonSerializer.Deserialize<List<Examen>>(responseString, serializeOptions);

                    return examenes;
                }
                else
                {
                    return new List<Examen>();
                }
            }
        }

        public async Task<bool> AgregarExamenApi(Examen examen)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(url);

                var response = await client.PostAsJsonAsync("Examenes", examen);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> ActualizarExamenApi(Examen examen)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(url);

                var response = await client.PutAsJsonAsync($"Examenes/{examen.IdExamen}", examen);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> EliminarExamenApi(int Id)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(url);

                var response = await client.DeleteAsync($"Examenes/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
