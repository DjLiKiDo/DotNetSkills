## General Coding Principles

1. **Single Responsibility**  
   Every module, class, or function must have one clear purpose.

2. **Open/Closed**  
   Write code so it is open to extension, but closed to modification.

3. **Keep it Simple (KISS)**  
   Prefer simple, straightforward solutions. Avoid unnecessary complexity.

4. **Don't Repeat Yourself (DRY)**  
   No copy-paste! Extract and reuse common code.

5. **You Aren't Gonna Need It (YAGNI)**  
   Only implement what is needed now. Avoid speculative features.

6. **Separation of Concerns**  
   Divide logic by responsibility. Each part should focus on one thing.

7. **Law of Demeter**  
   Use only direct collaborators. Don't chain calls or expose internals.

8. **Prefer Composition Over Inheritance**  
   Use composition where possible. Inherit only for real “is-a” relationships.

9. **Fail Fast**  
   Detect and report errors as early as possible.

10. **Tell, Don't Ask**  
    Prefer giving instructions to objects, not requesting data to make decisions outside.

---

## Code Quality & Maintainability

1. **Readable Code**  
   Code must be self-explanatory. Use clear names and structure.

2. **Explicitness**  
   Make behavior obvious. Avoid hidden side effects and magic values.

3. **Small Functions and Classes**  
   Functions/classes should do one thing and be short.

4. **Continuous Refactoring**  
   Always improve code. No code is “finished.”

5. **Automated Tests**  
   Cover business logic with automated tests. Test early and often.

---

## Security & DevOps

1. **Principle of Least Privilege**  
   Components must only access what they strictly need.

2. **Automate Everything**  
   Automate builds, tests, and deployments.

3. **Infrastructure as Code**  
   Treat infrastructure configuration as versioned code.

---

## What to Avoid

1. **God Objects**  
   No class should “know too much” or control everything.

2. **Spaghetti Code**  
   No tangled, hard-to-follow control flow.

3. **Premature Optimization**  
   Don’t optimize until necessary and measurable.

4. **Cargo Cult Programming**  
   No blind copy-paste or patterns without understanding why.

---

## Useful Links

- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Zen of Python](https://peps.python.org/pep-0020/)
- [Clean Code](https://www.oreilly.com/library/view/clean-code/9780136083238/)
- [GRASP Patterns](https://en.wikipedia.org/wiki/GRASP_(object-oriented_design))

---

> **Note:**  
> These rules are global. If you need to break a rule, justify it in code comments and discuss it with the team.
