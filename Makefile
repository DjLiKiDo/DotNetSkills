SHELL := /bin/zsh

.DEFAULT_GOAL := help

COMPOSE := docker compose -f $(CURDIR)/docker-compose.yml

.PHONY: help up down restart logs status build test health ps full cache

help: ## Show available targets
	@echo "Available targets:" && \
	grep -E '^[a-zA-Z_-]+:.*?## ' $(MAKEFILE_LIST) | sed -E 's/:.*## /\t- /' | sed 's/^/make /'

up: ## Start stack (API+DB)
	$(COMPOSE) up -d

down: ## Stop and remove containers (keep volumes)
	$(COMPOSE) down

restart: ## Clean restart using helper script
	./Docker/scripts/restart.sh --clean

logs: ## Tail API logs
	./Docker/scripts/logs.sh api --follow

status: ## Show environment status
	./Docker/scripts/status.sh

build: ## Restore and build solution (Release)
	dotnet restore ./DotNetSkills.sln && dotnet build ./DotNetSkills.sln -c Release

test: ## Run all tests (Release)
	dotnet test ./DotNetSkills.sln -c Release --no-build

health: ## Check API health endpoint (expects API running)
	curl -s -o /dev/null -w "%{http_code}\n" http://localhost:8080/health

ps: ## Show compose services
	$(COMPOSE) ps

full: ## Start all services including redis (compose profiles: cache, full)
	$(COMPOSE) --profile cache --profile full up -d

cache: ## Start only redis profile (useful for local testing)
	$(COMPOSE) --profile cache up -d redis
