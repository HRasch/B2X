---
docid: KB-193
title: Yaml Formatting
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# YAML Formatting — Quick Reference

Sources:
- YAML specification: https://yaml.org/spec/1.2/spec.html
- YAML project: https://yaml.org/
- yamllint: https://yamllint.readthedocs.io/
- Ansible YAML guide (practical gotchas): https://docs.ansible.com/

Summary (practical rules and tips)

- Indentation: Always use spaces, never tabs. Use a consistent indent width (2 spaces recommended for configs; 2 or 4 is fine for projects—pick one and be consistent).

- Document start/end: You may optionally start documents with `---` and end with `...` when multiple documents are in one file.

- Mappings (dictionaries): Keys and values are separated by `:` followed by a space. Example:

  ```yaml
  person:
    name: Alice
    age: 30
  ```

- Sequences (lists): Use `- ` at the same indentation level for each item.

  ```yaml
  fruits:
    - apple
    - banana
  ```

- Quoting: If a scalar contains characters that are syntactically significant (colon+space `: `, leading `-`, `?`, `{`, `[`, `|`, `>`, `&`, `*`, `#`, `!`, `%`, `@`, or commas in flow style), quote it. Use single quotes for literal strings without escapes, double quotes when you need escape sequences.

  - Prefer explicit quotes for values like `yes`, `no`, `on`, `off`, `true`, `false`, `null`, or values that look numeric (e.g., `001`, `1.0`) when you want them treated as strings.

- Booleans and magic values: YAML parsers sometimes coerce `yes/no`, `on/off`, `y/n`, `true/false`. To avoid surprises use `true`/`false` (lowercase) or quote the value to force a string.

- Numeric-like strings: If you need to preserve formatting (versions, leading zeros), quote them: `version: "1.0"` or `zip: "01234"`.

- Multiline values: Use block scalars:
  - Literal (`|`) preserves newlines.
  - Folded (`>`) folds newlines to spaces (useful for long paragraphs).

  Example:
  ```yaml
  literal: |
    line1
    line2

  folded: >
    this is a single logical line
    split across many physical lines
  ```

- Anchors & aliases: You can reuse values with `&anchor` and `*alias`. Use sparingly and document usage for readability.

- Avoid duplicate keys: Many parsers accept duplicates but semantics are ambiguous—use yamllint to enforce no-duplicates (`key-duplicates` rule).

- Flow vs block style: Prefer block style for readability. Flow style (`{a:1, b:2}` or `[1,2,3]`) is compact but harder to maintain.

- Comments: Start comments with `#`. Keep comments aligned with the data they explain.

- Trailing spaces & line length: Use a linter to enforce no trailing spaces and desired max line length.

Linting & tooling

- Use `yamllint` to check both syntax and style rules (indentation, trailing spaces, line-length, duplicate keys, quoting rules, etc.).
- Integrate `yamllint` with `pre-commit` and CI (GitHub Actions/GitLab) to block malformed/unclean YAML.
- Editor support: enable YAML extension in VS Code, enable schema validation where applicable (JSON Schema, Kwalify, or language-specific schemas).
- For round-tripping with comments, consider `ruamel.yaml` (Python) which preserves comments and ordering.

Common gotchas

- Unquoted strings that look like booleans or numbers will be interpreted by many parsers—quote when in doubt.
- A colon `:` inside an unquoted scalar followed by a space is treated as a mapping separator—quote values that contain `: `.
- Leading `-` or `?` characters may be parsed specially—quote if you mean a literal string starting with those characters.
- In templates (e.g., Jinja/`{{ }}`) quote whole value to avoid YAML parsing interpreting the braces as mapping start.

Examples

Valid (clear):
```yaml
server:
  host: "example.com"
  port: 8080
  features:
    - auth
    - metrics
```

Problematic (avoid):
```yaml
flag: yes    # may be interpreted as boolean
version: 1.0 # numeric -> parser may treat as float
path: c:\windows\system32  # backslashes, quote or use forward slashes
message: Hello: world  # colon unquoted -> parse error
```

Fixes (explicit):
```yaml
flag: "yes"
version: "1.0"
path: "c:\\windows\\system32"
message: 'Hello: world'
```

References

- YAML 1.2 spec: https://yaml.org/spec/1.2/spec.html
- YAML project: https://yaml.org/
- yamllint docs: https://yamllint.readthedocs.io/
- Ansible YAML guide (practical gotchas): https://docs.ansible.com/

Notes

- This is a practical summary for engineers editing configuration YAML files. For protocol-level correctness and advanced parsing behavior refer to the YAML 1.2 specification and your language's YAML library docs.
