# Contributing to ComboCounter

Thank you for your interest in contributing to ComboCounter! This document provides guidelines and instructions for contributing.

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on what is best for the community
- Show empathy towards other community members

## How to Contribute

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates. When creating a bug report, include:

- A clear and descriptive title
- Steps to reproduce the issue
- Expected behavior vs actual behavior
- Screenshots or videos if applicable
- Your Raft version and RaftModLoader version
- Any error messages from the console

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, include:

- A clear and descriptive title
- Detailed description of the proposed feature
- Explanation of why this enhancement would be useful
- Possible implementation approach (optional)

### Pull Requests

1. Fork the repository
2. Create a new branch from `main`:
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. Make your changes:
   - Follow the existing code style
   - Add comments for complex logic
   - Update documentation if needed
   - Test your changes thoroughly

4. Commit your changes:
   ```bash
   git commit -m "Add feature: description of your changes"
   ```

5. Push to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```

6. Open a Pull Request with:
   - Clear title and description
   - Reference to related issues
   - Screenshots/videos for UI changes

## Development Setup

1. Install prerequisites:
   - Visual Studio 2019 or later
   - .NET Framework 4.8
   - Raft game
   - RaftModLoader

2. Clone your fork:
   ```bash
   git clone https://github.com/YOUR_USERNAME/combocounter-raft.git
   cd combocounter-raft
   ```

3. Update reference paths in `ComboCounter/ComboCounter.csproj` to match your Raft installation

4. Build and package:
   ```bash
   build.bat
   ```

## Code Style Guidelines

- Use meaningful variable and method names
- Add XML documentation comments for public methods
- Keep methods focused and concise
- Handle exceptions appropriately
- Use `try-catch` blocks for error-prone operations
- Log errors with descriptive messages using `Debug.LogError`
- Do not add informational `Debug.Log` calls in production code
