# Feedback by Type

Feedback entries organized by issue type.

**Issue Types**:

### problems/
Bug reports, errors, unexpected behaviors, limitations
- Example: Build timing issues, encryption gaps, unclear behavior
- Owner: Typically the affected agent
- Resolution: Usually requires instruction update

### conflicts/
Disagreements between agents, unclear authority, process conflicts
- Example: Two agents created docs in different folders
- Owner: Both agents involved + @process-assistant
- Resolution: Clarify authority, update governance

### unclear-behavior/
Ambiguous instructions, confusing documentation, undocumented patterns
- Example: Wolverine pattern not fully documented
- Owner: @documentation-developer → @scrum-master
- Resolution: Add examples, clarify instruction

**Organization**:
```
by-type/
├── problems/
│   ├── 2025-12-30-build-timing-unclear.md
│   ├── 2025-12-31-encryption-key-rotation.md
│   └── summary.md
├── conflicts/
│   ├── 2025-12-30-documentation-authority.md
│   └── summary.md
└── unclear-behavior/
    ├── 2025-12-30-wolverine-routing.md
    └── summary.md
```

Each folder has a monthly `summary.md` created by @scrum-master with patterns and trends.

---

**Last Updated**: 30. Dezember 2025
