# Context Engineer Specification

## Purpose
Define the role, responsibilities, and operational patterns for a "Context Engineer" system that powers advanced GitHub Copilot Chat mode features (context awareness, retrieval, generation quality, learning, and safety) inside the DotNetSkills project lifecycle.

## Vision
Deliver high‑fidelity, minimal, and safe context to AI models—maximizing relevance, reducing hallucinations, and accelerating developer productivity while reinforcing domain & architectural standards.

## Core Objectives
1. Precision: Provide only the most relevant shards of code, docs, and history.
2. Adaptivity: Dynamically adjust context to intent, repo scale, and user profile.
3. Safety: Prevent secret leakage, license conflicts, and unsafe suggestions.
4. Traceability: Every answer is explainable (why each context element was selected).
5. Learning: Improve over time through telemetry and feedback loops.

## Responsibility Areas
| Area | Description | Key Artifacts |
| ---- | ----------- | ------------- |
| Context Acquisition | Index & update code, docs, symbols, tests | Embedding index, symbol graph |
| Retrieval Orchestration | Rank & assemble per-intent context bundles | Ranking pipeline, scoring logs |
| Prompt Assembly | Structured, layered prompt scaffolds | Prompt templates |
| Safety & Compliance | Redaction, license alerts, policy enforcement | Redaction rules, audit log |
| Interaction Intelligence | Track refinement loops & user preferences | Session profile store |
| Continuous Evaluation | Measure precision, latency, acceptance | KPI dashboards |

## Context Sources (Priority Tiers)
1. Ephemeral: selection, cursor scope, unsaved diff
2. Working Set: open editors, recent modified files
3. Structural: symbol graph (types, methods, call edges)
4. Behavioral: test code & failing traces
5. Historical: relevant commit messages & blame spans
6. Persistent Index: embedded code/doc chunks with metadata
7. External (policy‑gated): approved org knowledge, web/docs

## Ranking Signals
- Semantic similarity (vector cosine)
- Lexical relevance (BM25 / term weighting)
- Symbol proximity (call depth, ownership)
- Recency decay (exponential)
- Diversity (penalize near-duplicate embeddings)
- Confidence uplift (tests/examples prioritized for explain/test intents)

Final score = w1*semantic + w2*lexical + w3*symbol + w4*recency - w5*dupPenalty + intentBoost.

## Token Budget Strategy
Default allocation (adaptive):
- 40% Core code (targeted functions/classes)
- 25% Structural context (interfaces, contracts, signatures)
- 15% Tests/examples
- 10% Domain & architectural guidelines
- 10% Documentation excerpts
Fallback: shrink long code blocks via summarization (retain signatures + critical invariants).

## Intent Taxonomy
| Intent | Purpose | Special Handling |
| ------ | ------- | ---------------- |
| generate | Create new code | Emphasize interfaces + patterns |
| refactor | Improve existing code | Include before/after metrics & related tests |
| explain | Clarify behavior | Provide dependency chain & invariants |
| test | Generate/augment tests | Surface public API + edge cases list |
| debug | Diagnose faults | Include stack trace & suspect regions |
| optimize | Performance focus | Add hot paths + complexity/alloc hints |
| doc | Produce docs | Show signatures + domain rules |
| query | Repository Q&A | Favor higher-level modules & README |

## Prompt Layering Order
1. System guardrails (non-editable)
2. Project standards (DDD, Clean Architecture rules)
3. Intent header + user query
4. Curated context bundle (ranked slices with provenance tags)
5. Constraints (style, verbosity, risk mode)
6. Output contract instructions

## Provenance Tagging Format
`[[src:File.cs#L20-54|symbol:OrderService.CalculateTotals|score:0.82|tier:core]]`

## Safety & Compliance Layer
- Secret detection (regex + entropy + custom allowlist)
- License similarity (fingerprint hash match threshold >=80%)
- Redaction token: `****SECRET_TYPE(hash8)****`
- Policy gating: web/doc domains & risk modes
- Code reference disclosure when triggered

## Self-Critique Phases
1. Consistency: Does code compile vs referenced signatures?
2. Completeness: Are edge cases & null paths addressed?
3. Safety: Potential injection, race, resource leak?
4. Performance: Obvious inefficiencies flagged (O(n^2) in critical path)
5. Style Conformance: Align with project conventions (async suffix, value objects)

## Diff Packaging Standard
```
--- path/Old
+++ path/New
@@ lines
<changes>
```
Header comment (<=120 chars) summarizing rationale.

## Telemetry Events
| Event | Fields |
| ----- | ------ |
| RETRIEVE_START | intent, queryId, timestamp |
| RETRIEVE_RESULT | queryId, bundleId, items[], latencyMs |
| PROMPT_BUILT | bundleId, tokenCount, riskFlags |
| MODEL_RESPONSE | responseId, model, latencyMs, sizeTokens |
| USER_ACTION | responseId, actionType(apply/reject/refine), editDistance |
| SAFETY_FLAG | responseId, flagType(secret/license/security), severity |

## Key Metrics
- Precision@K (human-labeled relevance)
- Suggestion Acceptance Rate
- Post-Accept Edit Distance
- Retrieval Latency P50/P90
- Secret False Positive Rate
- Hallucination Incident Rate (reported)
- Learning Uplift (reduction in repeat ‘explain’ requests per symbol)

## Personalization Signals
- Rejection reasons (style mismatch, complexity, domain misalignment)
- Preferred idioms (.NET async/await, LINQ patterns)
- Verbosity preference drift
Decay outdated signals (half-life 14 days).

## Failure Modes & Fallbacks
| Failure | Fallback |
| ------- | -------- |
| Index missing | On-demand embed hotspot files only |
| Token overflow | Summarize least-relevant tail chunks |
| Secret detection block | Return masked result + guidance |
| Low confidence (< threshold) | Ask clarifying question (single) |
| Web blocked policy | Provide local-only answer disclaimer |

## Roadmap (Phases)
1. Baseline retrieval + diff generation
2. Multi-signal ranking + provenance tags
3. Self-critique + safety gates
4. Personalization + adaptive token budgeting
5. Predictive coaching & proactive insights

## Governance
- Weekly precision sampling (10 random sessions)
- Monthly safety audit (license & secret handling)
- Drift monitoring: threshold alerts on acceptance or latency degradation >10%

## Minimal System Prompt Skeleton
```
You are the Context Engineer for this repository. Goals: maximize relevance, minimize noise, enforce safety.
Project standards: <DDD/Clean Architecture summary>.
Honor user profile: <JSON knobs>.
Never fabricate paths; cite each context slice with provenance tags.
If confidence low: request clarification once.
Output format rules: <diff/doc/code contract>.
```

## Acceptance Definition
A response is considered context-engineered when:
- ≥85% of included slices are rated relevant
- No unredacted secret leakage
- Token budget within ±10% target
- Self-critique performed (meta markers present)

---
Maintained as a living document; update with each major retrieval or safety pipeline change.
