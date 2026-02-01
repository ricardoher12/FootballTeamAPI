# 📄 Project Write-Up

## 🧭 Overview & Approach

The goal of this project was to create a web application capable of ingesting, validating, and querying CSV data in a reliable and user-friendly way.

My approach focused on **data integrity**, **clear feedback**, and **defensive programming**.

The application allows users to upload a CSV file, which is first validated to ensure that **all required headers are present**. If any expected headers are missing, the request is rejected early to prevent partial or inconsistent data processing.

Once the headers are validated, the application processes the CSV **row by row**. Each row is independently validated and parsed. Valid rows are persisted to the database, while invalid rows are skipped and added to an **error collection**. This ensures that a small number of invalid records do not block the entire import process.

The API response includes:
- Number of successfully inserted records
- Number of failed records
- A list of row-level errors (including row number and error description)
- The successfully processed records

This design provides transparency to the client, allowing users to clearly identify which rows failed and why.

For data searching, the API exposes a filtering endpoint that accepts a **column name** and a **search value**. Based on the selected column’s data type (string, numeric, or date), the backend dynamically builds a safe Entity Framework query. Defensive validation is applied to avoid invalid or overly broad queries (such as wildcard-only searches).

---

## 🔄 Improvements & Potential Changes

If given additional time, I would consider implementing the following improvements:

- **Pagination and server-side sorting** to better support large datasets.
- **Batch inserts** during CSV processing to improve database performance.
- **Better requirements** so that team can make sure to build the right product

---

## 👀 Observations & UI Improvements

During development, it became clear that CSV imports are prone to many subtle data issues. Providing **row-level error reporting** significantly improves usability and troubleshooting.

The filtering interface could be further improved by:
- Restricting invalid filter combinations
- Adding autocomplete or suggestions for searchable values
- Preserving filter state between requests

---

## ⏱️ Time Spent

The total time spent coding the application (excluding documentation) was approximately **3 hours**.  
This includes backend API development, CSV validation logic, database integration, frontend implementation, and debugging.

---

## ✅ Summary

This project prioritizes correctness, resilience, and transparency. By validating data early, handling partial failures gracefully, and returning meaningful feedback to the client, the application provides a robust foundation for working with structured CSV data at scale.
