.PHONY: dev-up dev-down dev-logs

dev-up:
	docker compose -f docker-compose.dev.yaml up -d

dev-down:
	docker compose -f docker-compose.dev.yaml down

dev-logs:
	docker compose -f docker-compose.dev.yaml logs -f app

dev-rebuild:
	docker compose -f docker-compose.dev.yaml up -d --build

dev-restart:
	docker compose -f docker-compose.dev.yaml down && docker compose -f docker-compose.dev.yaml up -d --build
