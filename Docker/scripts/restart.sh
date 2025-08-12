#!/bin/bash

# ==============================================================================
# DotNetSkills Local Environment Restart Script
# ==============================================================================
# This script provides options to restart the local Docker environment with
# various configurations for different development scenarios.
# Usage: ./restart.sh [option]
# Options:
#   --clean            Clean restart (removes containers and starts fresh)
#   --full-clean       Complete clean restart (removes everything and rebuilds)
#   --db-only          Start database service only
#   --no-cache         Rebuild images without cache
#   --help             Show this help message

set -euo pipefail

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Project configuration
PROJECT_NAME="dotnetskills"
COMPOSE_FILE="docker-compose.yml"
ENV_FILE=".env"

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check prerequisites
check_prerequisites() {
    print_status "Checking prerequisites..."
    
    # Check Docker
    if ! command -v docker &> /dev/null; then
        print_error "Docker is not installed or not in PATH"
        exit 1
    fi
    
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running or not accessible"
        exit 1
    fi
    
    # Check Docker Compose
    if ! command -v docker-compose &> /dev/null; then
        print_error "Docker Compose is not installed or not in PATH"
        exit 1
    fi
    
    # Check compose file
    if [ ! -f "$COMPOSE_FILE" ]; then
        print_error "docker-compose.yml not found in current directory"
        exit 1
    fi
    
    print_success "Prerequisites check passed"
}

# Function to check and create .env file if needed
setup_environment() {
    if [ ! -f "$ENV_FILE" ]; then
        if [ -f ".env.example" ]; then
            print_status "Creating .env file from .env.example..."
            cp .env.example .env
            print_warning "Please review and update .env file with your specific configuration"
        else
            print_status "Creating minimal .env file..."
            cat > .env << EOF
# DotNetSkills Environment Configuration
# Generated on $(date)

# Environment
ASPNETCORE_ENVIRONMENT=Development

# Database Configuration
DB_SA_PASSWORD=DevPassword123
DB_NAME=DotNetSkills_Dev
DB_PORT=1433

# API Configuration
API_PORT=8080

# Redis Configuration (optional)
REDIS_PASSWORD=DevRedisPassword123!
REDIS_PORT=6379

# JWT Configuration
JWT_ENABLED=true
JWT_SIGNING_KEY=ThisIsATemporaryDevelopmentKeyForJWTSigningThatIsAtLeast256BitsLongForHS256AlgorithmSecurityRequirements

# Performance Monitoring
PERF_SLOW_THRESHOLD=500
PERF_VERY_SLOW_THRESHOLD=2000

# Migration Settings
RUN_MIGRATIONS=true
SEED_DATA=true
EOF
            print_success "Created minimal .env file"
        fi
    else
        print_status "Using existing .env file"
    fi
}

# Function to show help
show_help() {
    cat << EOF
DotNetSkills Local Environment Restart Script

Usage: ./restart.sh [OPTION]

OPTIONS:
    --clean             Clean restart (stops containers and starts fresh)
    --full-clean        Complete clean restart (removes volumes, images, rebuilds everything)
    --db-only           Start only the database service (for API development outside Docker)
    --cache-only        Start with Redis cache enabled
    --full              Start all services including optional ones (Redis cache)
    --no-cache          Rebuild Docker images without using cache
    --help              Show this help message

PROFILES:
    Default: Starts API and Database services
    Cache:   Includes Redis cache service (use --cache-only or --full)

EXAMPLES:
    ./restart.sh                 # Standard restart with API and DB
    ./restart.sh --clean         # Clean restart (safe, keeps data)
    ./restart.sh --full-clean    # Complete fresh start (⚠️  data loss)
    ./restart.sh --db-only       # Database only for local API development
    ./restart.sh --full          # All services including Redis cache

ENVIRONMENT VARIABLES:
    Create a .env file to customize configuration or copy from .env.example

For more information, see: docs/SECURITY-DEPLOYMENT.md
EOF
}

# Function for standard restart
standard_restart() {
    print_status "Performing standard restart..."
    
    # Stop any running services
    docker-compose down 2>/dev/null || true
    
    # Start core services (api + db)
    docker-compose up -d db api
    
    print_success "Standard restart completed"
    show_service_status
}

# Function for clean restart
clean_restart() {
    print_status "Performing clean restart..."
    
    # Stop and remove containers
    docker-compose down --remove-orphans
    
    # Start services fresh
    docker-compose up -d db api
    
    print_success "Clean restart completed"
    show_service_status
}

# Function for full clean restart
full_clean_restart() {
    print_warning "⚠️  This will remove all data and rebuild everything!"
    read -p "Are you sure? Type 'YES' to continue: " confirmation
    
    if [ "$confirmation" != "YES" ]; then
        print_status "Full clean restart cancelled"
        return
    fi
    
    print_status "Performing full clean restart..."
    
    # Use cleanup script if available
    if [ -f "Docker/scripts/cleanup.sh" ]; then
        bash Docker/scripts/cleanup.sh --full
    else
        # Manual cleanup
        docker-compose down --volumes --remove-orphans
        docker volume prune -f
        docker system prune -f
    fi
    
    # Rebuild and start
    docker-compose build --no-cache
    docker-compose up -d db api
    
    print_success "Full clean restart completed"
    show_service_status
}

# Function to start database only
start_db_only() {
    print_status "Starting database service only..."
    
    docker-compose down api 2>/dev/null || true
    docker-compose up -d db
    
    print_success "Database service started"
    print_status "API connection string: Server=localhost,1433;Database=DotNetSkills_Dev;User Id=sa;Password=DevPassword123;TrustServerCertificate=True"
    show_service_status
}

# Function to start with cache
start_with_cache() {
    print_status "Starting services with Redis cache..."
    
    docker-compose --profile cache up -d
    
    print_success "All services with cache started"
    show_service_status
}

# Function to start all services
start_full() {
    print_status "Starting all services..."
    
    docker-compose --profile full up -d
    
    print_success "All services started"
    show_service_status
}

# Function to rebuild without cache
rebuild_no_cache() {
    print_status "Rebuilding images without cache..."
    
    docker-compose down
    docker-compose build --no-cache
    docker-compose up -d db api
    
    print_success "Rebuild completed"
    show_service_status
}

# Function to show service status
show_service_status() {
    echo ""
    print_status "Service Status:"
    docker-compose ps
    
    echo ""
    print_status "Service URLs:"
    echo "  API:           http://localhost:8080"
    echo "  API Health:    http://localhost:8080/health"
    echo "  Swagger UI:    http://localhost:8080/swagger"
    echo "  Database:      localhost:1433 (sa/DevPassword123)"
    
    # Check if Redis is running
    if docker-compose ps redis | grep -q "Up"; then
        echo "  Redis Cache:   localhost:6379"
    fi
    
    echo ""
    print_status "Useful Commands:"
    echo "  Logs:          docker-compose logs -f"
    echo "  API Logs:      docker-compose logs -f api"
    echo "  DB Logs:       docker-compose logs -f db"
    echo "  Stop All:      docker-compose down"
    echo "  Cleanup:       ./Docker/scripts/cleanup.sh"
}

# Function to wait for services
wait_for_services() {
    print_status "Waiting for services to be ready..."
    
    # Wait for database
    timeout=60
    while ! docker-compose exec -T db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P DevPassword123 -C -Q "SELECT 1" > /dev/null 2>&1; do
        if [ $timeout -le 0 ]; then
            print_error "Database failed to start within timeout"
            return 1
        fi
        sleep 2
        timeout=$((timeout - 2))
        echo -n "."
    done
    
    echo ""
    print_success "Database is ready"
    
    # Wait for API if it's running
    if docker-compose ps api | grep -q "Up"; then
        timeout=60
        while ! curl -f http://localhost:8080/health > /dev/null 2>&1; do
            if [ $timeout -le 0 ]; then
                print_warning "API health check timeout (this may be expected)"
                break
            fi
            sleep 2
            timeout=$((timeout - 2))
            echo -n "."
        done
        
        if curl -f http://localhost:8080/health > /dev/null 2>&1; then
            echo ""
            print_success "API is ready"
        fi
    fi
}

# Main execution function
main() {
    check_prerequisites
    
    # Navigate to project root (assumes script is in Docker/scripts/)
    cd "$(dirname "$0")/../.."
    
    setup_environment
    
    # Parse command line arguments
    case "${1:-}" in
        --clean)
            clean_restart
            wait_for_services
            ;;
        --full-clean)
            full_clean_restart
            wait_for_services
            ;;
        --db-only)
            start_db_only
            wait_for_services
            ;;
        --cache-only)
            start_with_cache
            wait_for_services
            ;;
        --full)
            start_full
            wait_for_services
            ;;
        --no-cache)
            rebuild_no_cache
            wait_for_services
            ;;
        --help|-h)
            show_help
            ;;
        "")
            standard_restart
            wait_for_services
            ;;
        *)
            print_error "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
}

# Execute main function with all arguments
main "$@"