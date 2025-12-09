import React, { useEffect, useState } from "react";

const API_URL = "https://localhost:7142/api/employees"; // port number can be changed

export default function Employees() {
  const [employees, setEmployees] = useState([]);
  const [name, setName] = useState("");
  const [value, setValue] = useState("");
  const [editName, setEditName] = useState(""); // name of employee being edited

  // Load employees from backend
  useEffect(() => {
    loadEmployees();
  }, []);

  const loadEmployees = async () => {
    const res = await fetch(API_URL);
    const data = await res.json();
    setEmployees(data);
  };

  // Add new employee
  const addEmployee = async () => {
    if (!name) return alert("Name is required");
    await fetch(API_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name, value: parseInt(value) || 0 }),
    });
    setName("");
    setValue("");
    loadEmployees();
  };

  // Update employee (by name)
  const updateEmployee = async () => {
    if (!editName) return;
    await fetch(`${API_URL}/${editName}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name, value: parseInt(value) || 0 }),
    });
    setEditName("");
    setName("");
    setValue("");
    loadEmployees();
  };

  // Delete employee (by name)
  const deleteEmployee = async (name) => {
    if (!window.confirm(`Delete employee ${name}?`)) return;
    await fetch(`${API_URL}/${name}`, { method: "DELETE" });
    loadEmployees();
  };

  // Increment values (SQL button)
  const incrementValues = async () => {
    await fetch(`${API_URL}/increment-values`, { method: "POST" });
    loadEmployees();
  };

  // ABC Sum
  const getABCSum = async () => {
    const res = await fetch(`${API_URL}/abc-sum`);
    const data = await res.text();
    alert("ABC Sum Result: " + data);
  };

  return (
    <div style={{ padding: 20 }}>
      <h2>Employees</h2>

      <input
        placeholder="Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
      />
      <input
        placeholder="Value"
        value={value}
        onChange={(e) => setValue(e.target.value)}
      />

      {editName ? (
        <button onClick={updateEmployee}>Update</button>
      ) : (
        <button onClick={addEmployee}>Add</button>
      )}

      <hr />

      <button onClick={incrementValues}>Increment Values</button>
      <button onClick={getABCSum}>Get ABC Sum</button>

      <ul>
        {employees.map((e) => (
          <li key={e.Name}>
            {e.Name} — {e.Value}
            <button
              onClick={() => {
                setEditName(e.Name);
                setName(e.Name);
                setValue(e.Value);
              }}
            >
              Edit
            </button>
            <button onClick={() => deleteEmployee(e.Name)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
