using InterviewTest.Server.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace InterviewTest.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private string ConnectionString =>
            new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" }.ToString();

        // GET: List all employees
        [HttpGet]
        public List<Employee> Get()
        {
            var employees = new List<Employee>();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT Name, Value FROM Employees";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Name = reader.GetString(0),
                            Value = reader.GetInt32(1)
                        });
                    }
                }
            }

            return employees;
        }

        // POST: Add new employee
        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO Employees (Name, Value) VALUES (@name, @value)";
                cmd.Parameters.AddWithValue("@name", employee.Name);
                cmd.Parameters.AddWithValue("@value", employee.Value);
                cmd.ExecuteNonQuery();
            }
            return Ok();
        }

        // PUT: Update employee by Name
        [HttpPut("{name}")]
        public IActionResult Update(string name, Employee employee)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"UPDATE Employees SET Value=@value WHERE Name=@name";
                cmd.Parameters.AddWithValue("@value", employee.Value);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
            return Ok();
        }

        // DELETE employee by Name
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"DELETE FROM Employees WHERE Name=@name";
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
            return Ok();
        }

        // POST: Increment values by rule
        [HttpPost("increment-values")]
        public IActionResult IncrementValues()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Employees
                    SET Value =
                        CASE
                            WHEN Name LIKE 'E%' THEN Value + 1
                            WHEN Name LIKE 'G%' THEN Value + 10
                            ELSE Value + 100
                        END
                ";
                cmd.ExecuteNonQuery();
            }
            return Ok();
        }

        // GET: ABC sum >= 11171
        [HttpGet("abc-sum")]
        public IActionResult ABCTotal()
        {
            int sum = 0;

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SELECT SUM(Value) 
                    FROM Employees
                    WHERE Name LIKE 'A%'
                       OR Name LIKE 'B%'
                       OR Name LIKE 'C%'
                ";
                var result = cmd.ExecuteScalar();
                sum = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }

            if (sum >= 11171)
                return Ok(sum);

            return Ok("No results ≥ 11171");
        }
    }
}
