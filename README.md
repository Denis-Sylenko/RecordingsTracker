# **RecordingsTracker**

**RecordingsTracker** is a solution designed to monitor file creation events in specified folders. It provides notifications through Telegram when certain conditions are met, such as the creation of empty `.deflate` files or a prolonged absence of file creation.

---

## **Features**
- Monitors specified folders for file creation and deletion events.
- Sends notifications to a Telegram bot:
  - Warns if empty `.deflate` files are created.
  - Alerts if no files have been created for a specified period.
- Modular architecture with three separate projects:
  - **RecordingsTracker**: Core functionality for monitoring file events.
  - **NotifierBot**: Handles Telegram bot interactions, sending notifications.
  - **Core**: Centralized configuration and infrastructure management.

---

## **Projects Overview**

1. **RecordingsTracker**  
   - Listens for file creation and deletion in configured folders.  
   - Handles detected file events based on specified criteria.  

2. **NotifierBot**  
   - Interacts with Telegram's API to send warning messages.  
   - Designed to trigger alerts based on monitoring results.  

3. **Core**  
   - Provides configuration settings shared across projects.  
   - Manages infrastructure-level concerns.
