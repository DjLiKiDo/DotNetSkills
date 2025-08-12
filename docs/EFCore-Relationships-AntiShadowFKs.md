# EF Core Relationships: Anti-Shadow-FK Guideline

This guideline prevents EF Core from generating shadow foreign key columns like `TeamId1` / `UserId1` due to duplicate or ambiguous relationship mappings.

## Golden rules
- Configure each relationship exactly once. Avoid mixing conventions with partial explicit config.
- Prefer dependent navigations when the dependent entity exists (join entities):
  - `HasOne(dep => dep.Principal).WithMany(principal => principal.Collection).HasForeignKey(dep => dep.PrincipalId)`
- Don’t reconfigure the same relationship again from the principal side.
- Keep FK properties strongly-typed (ValueConverters) and align names with navigations.

## Join entity pattern (recommended)

Example: `TeamMember` between `User` and `Team`.

- Domain
  - Add dependent navigations on the join entity:
    - `public User User { get; private set; } = null!;`
    - `public Team Team { get; private set; } = null!;`
- Configuration
  - Map from the dependent using both navigations:
    - `HasOne(tm => tm.User).WithMany(u => u.TeamMemberships).HasForeignKey(tm => tm.UserId)`
    - `HasOne(tm => tm.Team).WithMany(t => t.Members).HasForeignKey(tm => tm.TeamId)`

This consolidates relationships and prevents EF from creating additional shadow FKs.

## When not using dependent navigations
If you decide not to expose dependent navigations, then map the relationship only from the principal side and remove any partial mapping from the dependent to avoid duplication.

- `Team` config: `HasMany(t => t.Members).WithOne().HasForeignKey(tm => tm.TeamId)`
- `User` config: `HasMany(u => u.TeamMemberships).WithOne().HasForeignKey(tm => tm.UserId)`

## Delete behaviors
- Define delete behaviors explicitly. Don’t rely on conventions.
- Example
  - `TeamMember` uses `Cascade` when deleting `User` or `Team`.
  - `Task.AssignedUserId` uses `SetNull`.
  - `Project.TeamId` uses `Restrict`.

## Strongly-typed IDs
- Always configure value converters for FK properties.
- Keep FK names aligned with navigation names: `UserId`, `TeamId`, etc.

## Anti-patterns (avoid)
- Mapping a relationship twice (once from dependent and once from principal) with different navigations, causing EF to infer extra FKs.
- Leaving conventions to infer relationships while also configuring FK properties separately.
- Exposing IQueryable from repositories (leaks persistence details).

## Verification checklist
- Model snapshot doesn’t include `*Id1` properties.
- Each relationship is configured in only one place.
- Constraint names, indexes, and delete behaviors are correct.
- Unit/integration tests cover basic CRUD paths.
