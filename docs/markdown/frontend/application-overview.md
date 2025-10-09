# Application Overview

## Introduction

The NextQuestionnaire application is a questionnaire system designed for educational institutions where both students and teachers answer questions, while administrators manage the questionnaire data without participating in responses.

## Core Concepts

### Questionnaire Templates
A template for questionnaires containing questions and answer options. Each question can have a toggle to allow custom answers, enabling users to provide user-defined responses. Before a template can be used, it must be set to a finalized state. Once finalized, the template can no longer be edited - if updates are needed, it must be copied, which duplicates all questions and options.

### Active Questionnaires/Groups
Active Questionnaires are instances of questionnaire templates that require both a student and teacher to be assigned. The system includes a grouping feature where multiple active questionnaires can be grouped together, allowing multiple active questionnaires to be generated simultaneously. However, they technically still count as separate entities for answering and results.

## User Roles and Permissions

**Student**: Can only answer assigned questionnaires and cannot view their own responses.

**Teacher**: Can answer assigned questionnaires and view the results of questionnaires.

**Administrator**: Can edit, create, and delete templates, and assign active questionnaires/groups. Administrators do not answer questionnaires.

## Application Workflow

### 1. Template Creation
Administrators create questionnaire templates with questions and answer options. Templates must be finalized before use.

### 2. Active Questionnaire Assignment
Administrators assign questionnaire templates to both students and teachers, creating active questionnaires. Multiple questionnaires can be grouped together for batch creation.

### 3. Response Collection
Both students and teachers complete their assigned questionnaires by answering the questions.

### 4. Results Review
Teachers can view and analyze the responses from questionnaires they participated in.

## Key Features

- **Template Management**: Create, edit, and finalize questionnaire templates
- **Custom Answers**: Allow users to provide custom responses beyond predefined options
- **Batch Assignment**: Group multiple questionnaires for simultaneous deployment
- **Role-based Access**: Different capabilities for students, teachers, and administrators
- **Response Analytics**: Teachers can review questionnaire results

## Navigation

**All Users**: 
- Home

**Students**: 
- Active Questionnaires

**Teachers**: 
- Overview (Teacher Dashboard)
- Active Questionnaires 
- Data Compare

**Administrators**: 
- Templates
- Active Questionnaires