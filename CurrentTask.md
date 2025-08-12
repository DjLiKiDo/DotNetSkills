<!-- Hotfix completed: .env purge and prevention (2025-08-12) -->

## Urgente: Remediación por commit de .env en rama devops-infra

Fecha: 2025-08-12 · Rama afectada: devops-infra · PR: #10 (Docker)

Contexto: Se detectaron archivos `.env` en historial. No se publicaron con secretos; se requiere purgar del historial y prevenir reintroducción.

Acciones ejecutadas:
- Inventario: `.env` y `.env.example` identificados en working tree e historial.
- Respaldo local: copia a `$HOME/DotNetSkills-env-backup-<timestamp>`.
- Limpieza: `git filter-repo --path .env --path .env.example --invert-paths` (eliminación completa en `devops-infra`).
- Prevención: reglas añadidas a `.gitignore` para `.env`, `.env.*`, `**/*.env`, `**/*.env.*`, `*.secrets.*`, `secrets/*.json`, `secrets/*.yaml`.
- Publicación: `git push --force-with-lease origin devops-infra` (PR #10 actualizado).

Verificación:
- `git ls-files` sin .env.
- `git log --all --name-only` sin rutas .env.

Checklist:
- [x] Purgar .env del historial
- [x] Conservar cambios funcionales
- [x] Prevenir reintroducción via .gitignore
- [x] Rama/PR actualizados

Notas: El force-push requiere realinear clones locales: `git fetch` y `git reset --hard origin/devops-infra`.

# Plan: Auditoría y remediación de Shadow FKs en EF Core

Fecha: 2025-08-12 · Rama: devops-infra · PR activo: Docker (#10)

## Contexto
Comentario de PR: El ModelSnapshot contiene propiedades FK sombra (TeamId1, UserId1), indicando configuraciones duplicadas de relaciones en EF Core. Esto suele ocurrir cuando EF detecta múltiples rutas de navegación y crea FKs adicionales. Aunque el refactor de TeamMemberConfiguration centraliza relaciones, el snapshot aún refleja estas propiedades sombra, lo que puede ocasionar problemas de esquema.

## Objetivo
Eliminar FKs sombra (p.ej. TeamId1, UserId1) asegurando que cada relación esté configurada una sola vez, con claves, delete behaviors y nombres de constraints correctos, y prevenir su reintroducción en el resto de bounded contexts.

## Alcance (bounded contexts y entidades)
- TeamCollaboration
	- TeamMember ↔ User (UserId)
	- TeamMember ↔ Team (TeamId)
	- Team ↔ Project (ya configurado)
- UserManagement
	- User ↔ TeamMember (colección TeamMemberships)
- ProjectManagement
	- Project ↔ Team (TeamId)
- TaskExecution
	- Task ↔ Project (ProjectId)
	- Task ↔ User (AssignedUserId nullable)
	- Task (self-reference) ParentTaskId/Subtasks

## Hallazgos iniciales (repo)
- Migrations/ModelSnapshot contienen TeamId1 y UserId1:
	- src/DotNetSkills.Infrastructure/Migrations/ApplicationDbContextModelSnapshot.cs
	- 20250811200320_InitialCreate.cs y Designer.cs
- Foto de relaciones en snapshot (resumen):
	- TeamMember → Team: se ven DOS relaciones: WithMany() con FK "TeamId" y WithMany("Members") con FK "TeamId1" (sombra).
	- TeamMember → User: se ven DOS relaciones: WithMany() con FK "UserId" y WithMany("TeamMemberships") con FK "UserId1" (sombra).
- En código actual:
	- TeamMemberConfiguration configura explícitamente ambas relaciones con HasOne(...).WithMany(nav).HasForeignKey(prop) y DeleteBehavior.Cascade.
	- TeamConfiguration/UserConfiguration NO reconfiguran estas relaciones (solo backing fields), por lo que la duplicidad proviene de convenciones + la configuración explícita.

Hipótesis de causa raíz
- EF crea por convención una relación basada en la presencia del FK (TeamId/UserId) y una navegación en el lado principal (Team.Members, User.TeamMemberships). Al añadir además la configuración explícita desde TeamMember sin navegación dependiente, EF no unifica y genera una relación adicional usando FKs sombra (TeamId1/UserId1).

## Criterios de aceptación
- [ ] No existen propiedades "*Id1" en el ModelSnapshot ni en migraciones nuevas.
- [ ] Solo hay UNA relación TeamMember→Team con FK TeamId y navegación principal Team.Members.
- [ ] Solo hay UNA relación TeamMember→User con FK UserId y navegación principal User.TeamMemberships.
- [ ] Delete behaviors, constraint names e índices permanecen correctos.
- [ ] Build y tests pasan; CRUD básico funciona para Team/User/TeamMember/Project/Task.
- [ ] Documentación para evitar reintroducir el problema.

## Plan de trabajo
1) Auditoría completa (snapshots y config)
	 - Listar todas las referencias a "*Id1" y relaciones duplicadas en el snapshot.
	 - Revisar configuraciones de entidades por bounded context para detectar posibles patrones similares.

2) Análisis y estrategia de fix
	 - Opción A (recomendada): Añadir navegaciones dependientes en TeamMember y mapear como:
		 - HasOne(tm => tm.Team).WithMany(t => t.Members).HasForeignKey(tm => tm.TeamId)
		 - HasOne(tm => tm.User).WithMany(u => u.TeamMemberships).HasForeignKey(tm => tm.UserId)
		 - Esto facilita a EF consolidar en una sola relación y evita creación por convención separada.
	 - Opción B: Mantener TeamMember sin navegaciones dependientes, pero definir las relaciones desde el lado principal (Team/User) con WithOne().HasForeignKey(...), y eliminar las definiciones en TeamMemberConfiguration para evitar duplicidad.
	 - Validar delete behaviors y constraint names tras el cambio.

3) Implementación
	 - Elegir opción (A o B) y aplicar cambios en:
		 - Domain: si Opción A, agregar navegaciones en TeamMember (propiedades de solo lectura para EF) sin exponer en API pública si se desea.
		 - Infrastructure: actualizar TeamMemberConfiguration (y si aplica, TeamConfiguration/UserConfiguration) conforme a la opción elegida.
	 - Regenerar migración correctiva para eliminar columnas/índices/constraints sombra (TeamId1/UserId1).

4) Validación end-to-end
	 - dotnet build, dotnet test.
	 - Aplicar migraciones en DB de desarrollo; probar alta/baja de TeamMember, Team, User, Project, Task.

5) Prevención
	 - Añadir guía en docs/ y/o comentarios en configuraciones para estandarizar patrón de mapeo de join entities y evitar mezcla de convención + configuración parcial.

## Checklist de tareas (tracking)
- [x] Auditoría de snapshots (detectar TeamId1/UserId1 y archivos afectados)
- [x] Revisar configuraciones (inventario de relaciones por bounded context)
- [x] Análisis de causa y elegir estrategia de fix (A/B)
- [x] Redactar cambios concretos por entidad (FKs, WithOne/WithMany, InverseProperty si aplica)
- [x] Plan de migración (drop shadow cols/idx/constraints + data safety)
- [x] Implementar fixes + crear migración
- [x] Validar build/tests y smoke tests de CRUD
- [x] Documentar guideline anti-shadow-FK

## Notas de implementación (borrador)
- Si se adopta Opción A:
	- Domain.TeamCollaboration.Entities.TeamMember
		- Agregar: `public Team Team { get; private set; } = null!;`
		- Agregar: `public User User { get; private set; } = null!;`
	- Infra TeamMemberConfiguration
		- Usar HasOne(tm => tm.Team).WithMany(t => t.Members).HasForeignKey(tm => tm.TeamId).HasConstraintName("FK_TeamMembers_Teams_TeamId").OnDelete(Cascade)
		- Usar HasOne(tm => tm.User).WithMany(u => u.TeamMemberships).HasForeignKey(tm => tm.UserId).HasConstraintName("FK_TeamMembers_Users_UserId").OnDelete(Cascade)
	- Verificar que TeamConfiguration/UserConfiguration no vuelvan a definir las mismas relaciones.

- Si se adopta Opción B:
	- Infra TeamMemberConfiguration: eliminar HasOne(...).WithMany(...) y solo configurar propiedades (UserId/TeamId), índices y constraints no-relacionales.
	- Infra TeamConfiguration: HasMany(t => t.Members).WithOne().HasForeignKey(tm => tm.TeamId)...
	- Infra UserConfiguration: HasMany(u => u.TeamMemberships).WithOne().HasForeignKey(tm => tm.UserId)...

## Riesgos/Edge cases
- Migraciones en entornos ya aplicadas: planificar script para drop columns TeamId1/UserId1 si existen con datos nulos (deberían estar nulos por ser sombra no usada).
- Delete behaviors: mantener Restrict/SetNull donde aplique (p.ej. Task.AssignedUserId usa SetNull; TeamMember usa Cascade al borrar Team/User según reglas actuales).
- Strongly-typed IDs: verificar conversiones y claves compuestas de índices únicos (UserId, TeamId) siguen intactas.

## Progreso y decisiones
- Estrategia elegida: Opción A (navegaciones dependientes en TeamMember) para consolidar relaciones y evitar FKs sombra.
- Cambios aplicados:
	- Domain.TeamMember: añadidas propiedades de navegación `User` y `Team` (solo para EF).
	- Infra.TeamMemberConfiguration: actualizado a `HasOne(tm => tm.User/Team)` + `WithMany(TeamMemberships/Members)` con `HasForeignKey` explícitos.
	- Generada migración `Fix_ShadowFKs_TeamMember` que elimina TeamId1/UserId1, índices y FKs asociados.
- Validación:
	- dotnet build: OK.
	- dotnet test (unit + infra): 34/34 OK. Nuevo test `TeamMemberMappingTests` valida ausencia de TeamId1/UserId1 y CRUD básico con InMemory.
	- `dotnet ef database update`: aplicada con éxito; columnas/índices/constraints sombra eliminados.

## Siguientes pasos inmediatos
1) Añadir breve nota en el PR con before/after del ModelSnapshot para visibilidad.
2) Considerar un test de integración con SQL Server (Testcontainers) para cubrir comportamiento de cascadas.
3) Cerrar la tarea una vez mergeado.

