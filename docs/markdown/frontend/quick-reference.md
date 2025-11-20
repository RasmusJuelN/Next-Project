# Quick Reference Guide

This guide provides quick access to common tasks and important information for all users of the NextQuestionnaire application.

## Quick Actions by Role

### Students
| Task | Location | Steps |
|------|----------|-------|
| View assigned questionnaires | Active Questionnaires | Login → Active Questionnaires |
| Complete a questionnaire | Active Questionnaires | Click questionnaire → Answer questions → Submit |
| Note: Students cannot view their submitted responses | | |

### Teachers
| Task | Location | Steps |
|------|----------|-------|
| Access teacher dashboard | Overview | Login → Overview |
| View assigned questionnaires | Active Questionnaires | Login → Active Questionnaires |
| Complete questionnaires | Active Questionnaires | Click questionnaire → Answer questions → Submit |
| View questionnaire results | Overview or Data Compare | Overview → Select questionnaire OR Data Compare |
| Analyze response data | Data Compare | Data Compare → Select questionnaire → View analysis |

### Administrators
| Task | Location | Steps |
|------|----------|-------|
| Create template | Templates | Templates → New Template → Add questions → Save/Finalize |
| Copy template for editing | Templates | Templates → Select template → Copy → Edit → Finalize |
| Create active questionnaire | Active Questionnaires | Active Questionnaires → Select template → Assign student & teacher |
| Create questionnaire groups | Active Questionnaires | Active Questionnaires → Group multiple questionnaires |

## Important Application URLs

### Main Application Sections
- **Home**: `/` 
- **Templates**: `/templates` (Administrators only)
- **Active Questionnaires**: `/active-questionnaire` (Administrators) or `/show-active-questionnaires` (Students/Teachers)

### Teacher-Specific Routes
- **Overview (Teacher Dashboard)**: `/teacher-dashboard`
- **Data Compare**: `/data-compare`

## Question Types Reference

### Available Question Types
| Type | Description | Usage |
|------|-------------|-------|
| **Multiple Choice** | Predefined options only | Users select from provided choices |
| **Custom Answer Only** | User-provided responses only | Users write their own answers |
| **Mixed** | Both predefined options AND custom answers | Users can choose from options OR provide custom response |

## Common Error Messages and Solutions

### Authentication Errors
| Error | Cause | Solution |
|-------|-------|----------|
| "Invalid credentials" | Wrong username/password | Verify credentials, contact admin if needed |
| "Access denied" | Insufficient permissions | Contact administrator to verify role assignment |
| "Session expired" | JWT token expired | Log out and log back in |
| "Unauthorized" | No valid authentication | Ensure you're logged in |

### Application Errors
| Error | Cause | Solution |
|-------|-------|----------|
| "Template not found" | Deleted or invalid template | Refresh page, contact administrator |
| "Template not finalized" | Trying to use draft template | Administrator must finalize template first |
| "Questionnaire expired" | Past deadline | Contact administrator |
| "Response already submitted" | Duplicate submission | Cannot resubmit |
| "Missing assignment" | Student/Teacher not assigned | Contact administrator to verify assignment |
