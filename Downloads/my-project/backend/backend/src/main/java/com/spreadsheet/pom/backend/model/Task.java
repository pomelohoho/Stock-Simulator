package com.spreadsheet.pom.backend.model;

import java.util.ArrayList;
import java.util.List;

public class Task {
    private String title;
    private String description;
    private String time;
    private boolean completed;

    // Default Constructor
    public Task() {
    }

    // Parameterized Constructor
    public Task(String title, String description, String time, boolean completed) {
        this.title = title;
        this.description = description;
        this.time = time;
        this.completed = completed;
    }

    // Getters and Setters
    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getTime() {
        return time;
    }

    public void setTime(String time) {
        this.time = time;
    }

    public boolean isCompleted() {
        return completed;
    }

    public void setCompleted(boolean completed) {
        this.completed = completed;
    }
}