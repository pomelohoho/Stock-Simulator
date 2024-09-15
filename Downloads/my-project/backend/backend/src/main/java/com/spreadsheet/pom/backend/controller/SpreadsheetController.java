package com.spreadsheet.pom.backend.controller;
import org.springframework.http.ResponseEntity;

import org.springframework.web.bind.annotation.*;
import java.util.ArrayList;
import java.util.List;

@RestController
@RequestMapping("/api")
public class SpreadsheetController {

    private List<String> tasks = new ArrayList<>(List.of("Task 1", "Task 2", "Task 3"));

    @GetMapping("/tasks")
    public List<String> getAllTasks() {
        return tasks;
    }

    @PostMapping("/tasks")
    public String addTask(@RequestBody String newTask) {
        tasks.add(newTask);
        return "Task added successfully!";
    }

    @PutMapping("/tasks/{index}")
    public String editTask(@PathVariable int index, @RequestBody String updatedTask) {
        if (index >= 0 && index < tasks.size()) {
            tasks.set(index, updatedTask);
            return "Task updated successfully!";
        } else {
            return "Invalid index!";
        }
    }

    @DeleteMapping("/tasks/{index}")
    public String deleteTask(@PathVariable int index) {
        if (index >= 0 && index < tasks.size()) {
            tasks.remove(index);
            return "Task deleted successfully!";
        } else {
            return "Invalid index!";
        }
    }
}