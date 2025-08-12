#!/bin/bash

# ==============================================================================
# DotNetSkills Environment Status Script
# ==============================================================================
# This script provides comprehensive status information about the local
# Docker environment including services, health, resources, and connectivity.
# Usage: ./status.sh [options]

set -euo pipefail

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m' # No Color

# Function to print colored output
print_header() {
    echo -e "${BOLD}${BLUE}=== $1 ===${NC}"
}

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}

print_value() {
    echo -e "${CYAN}  $1:${NC} $2"
}

# Function to show help
show_help() {
    cat << EOF
DotNetSkills Environment Status Checker

Usage: ./status.sh [OPTIONS]

OPTIONS:
    --services       Show only service status
    --health         Show only health check information
    --resources      Show only resource usage
    --connectivity   Show only connectivity tests
    --json          Output in JSON format
    --help          Show this help message

EXAMPLES:
    ./status.sh                  # Full status report
    ./status.sh --services       # Services status only
    ./status.sh --health         # Health checks only

The status script provides:
- Docker service status
- Container health checks
- Resource usage (CPU, Memory)
- Network connectivity
- Service endpoints availability
- Database connection status
- Environment configuration
EOF
}

# Function to check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running or not accessible"
        return 1
    fi
    return 0
}

# Function to show Docker system info
show_docker_info() {
    print_header "Docker System Information"
    
    if check_docker; then
        print_success "Docker is running"
        
        # Docker version
        DOCKER_VERSION=$(docker --version | cut -d ' ' -f3 | sed 's/,//')
        print_value "Docker Version" "$DOCKER_VERSION"
        
        # Docker Compose version
        if command -v docker-compose &> /dev/null; then
            COMPOSE_VERSION=$(docker-compose --version | cut -d ' ' -f4 | sed 's/,//')
            print_value "Docker Compose Version" "$COMPOSE_VERSION"
        else
            print_warning "Docker Compose not found"
        fi
        
        # System resources
        DOCKER_INFO=$(docker system df --format "table {{.Type}}\t{{.Total}}\t{{.Active}}\t{{.Size}}")
        echo ""
        print_status "Docker Resources:"
        echo "$DOCKER_INFO" | tail -n +2 | while IFS=$'\t' read -r type total active size; do
            print_value "$type" "Total: $total, Active: $active, Size: $size"
        done
    else
        return 1
    fi
    echo ""
}

# Function to show service status
show_service_status() {
    print_header "Service Status"
    
    if [ ! -f "docker-compose.yml" ]; then
        print_error "docker-compose.yml not found"
        return 1
    fi
    
    # Get service status
    local services_output
    services_output=$(docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}" 2>/dev/null || echo "")
    
    if [ -z "$services_output" ]; then
        print_warning "No services are currently running"
        echo ""
        return 0
    fi
    
    echo "$services_output" | tail -n +2 | while IFS=$'\t' read -r name status ports; do
        if echo "$status" | grep -q "Up"; then
            print_success "$name: $status"
            if [ -n "$ports" ]; then
                print_value "  Ports" "$ports"
            fi
        else
            print_error "$name: $status"
        fi
    done
    
    echo ""
}

# Function to check service health
check_service_health() {
    print_header "Health Check Status"
    
    # Check API health
    if docker-compose ps api | grep -q "Up"; then
        print_status "Checking API health..."
        if curl -f http://localhost:8080/health > /dev/null 2>&1; then
            local health_response
            health_response=$(curl -s http://localhost:8080/health)
            print_success "API health endpoint responding"
            echo "  Response: $health_response"
        else
            print_warning "API health endpoint not responding (this may be expected)"
        fi
    else
        print_warning "API service is not running"
    fi
    
    # Check database health
    if docker-compose ps db | grep -q "Up"; then
        print_status "Checking database health..."
        if docker-compose exec -T db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "${DB_SA_PASSWORD:-DevPassword123}" -C -Q "SELECT 1" > /dev/null 2>&1; then
            print_success "Database is responding to queries"
        else
            print_error "Database is not responding to queries"
        fi
    else
        print_warning "Database service is not running"
    fi
    
    # Check Redis health
    if docker-compose ps redis | grep -q "Up" 2>/dev/null; then
        print_status "Checking Redis health..."
        if docker-compose exec -T redis redis-cli ping | grep -q "PONG" 2>/dev/null; then
            print_success "Redis is responding to ping"
        else
            print_error "Redis is not responding to ping"
        fi
    else
        print_status "Redis service is not running (optional service)"
    fi
    
    echo ""
}

# Function to show resource usage
show_resource_usage() {
    print_header "Resource Usage"
    
    # Get container stats
    local running_containers
    running_containers=$(docker-compose ps -q 2>/dev/null | tr '\n' ' ')
    
    if [ -n "$running_containers" ]; then
        print_status "Container Resource Usage:"
        docker stats --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}" $running_containers | tail -n +2 | while IFS=$'\t' read -r container cpu mem net; do
            print_value "$container" "CPU: $cpu, Memory: $mem, Network: $net"
        done
    else
        print_warning "No containers are running"
    fi
    
    echo ""
}

# Function to test connectivity
test_connectivity() {
    print_header "Connectivity Tests"
    
    # Test service endpoints
    local endpoints=(
        "http://localhost:8080/health:API Health"
        "http://localhost:8080/swagger:API Documentation"
    )
    
    for endpoint_info in "${endpoints[@]}"; do
        IFS=':' read -r url description <<< "$endpoint_info"
        print_status "Testing $description ($url)..."
        
        if curl -f "$url" > /dev/null 2>&1; then
            print_success "$description is accessible"
        else
            print_warning "$description is not accessible"
        fi
    done
    
    # Test database connection
    if docker-compose ps db | grep -q "Up"; then
        print_status "Testing database connection..."
        
        if timeout 5 bash -c "cat < /dev/null > /dev/tcp/localhost/1433" 2>/dev/null; then
            print_success "Database port 1433 is reachable"
        else
            print_warning "Database port 1433 is not reachable"
        fi
    fi
    
    echo ""
}

# Function to show environment configuration
show_environment() {
    print_header "Environment Configuration"
    
    # Show .env file status
    if [ -f ".env" ]; then
        print_success ".env file exists"
        print_status "Key configuration values:"
        
        # Safe environment variables to display
        local safe_vars=("ASPNETCORE_ENVIRONMENT" "API_PORT" "DB_PORT" "DB_NAME" "RUN_MIGRATIONS" "SEED_DATA")
        
        for var in "${safe_vars[@]}"; do
            if grep -q "^$var=" .env; then
                local value
                value=$(grep "^$var=" .env | cut -d'=' -f2)
                print_value "$var" "$value"
            fi
        done
    else
        print_warning ".env file not found"
        if [ -f ".env.example" ]; then
            print_status ".env.example is available as a template"
        fi
    fi
    
    echo ""
}

# Function to show network information
show_network_info() {
    print_header "Network Information"
    
    # Check if project network exists
    if docker network ls | grep -q "dotnetskills-network"; then
        print_success "Project network 'dotnetskills-network' exists"
        
        # Show connected containers
        local connected_containers
        connected_containers=$(docker network inspect dotnetskills-network --format='{{range .Containers}}{{.Name}} {{end}}' 2>/dev/null || echo "")
        
        if [ -n "$connected_containers" ]; then
            print_value "Connected containers" "$connected_containers"
        else
            print_status "No containers currently connected to the network"
        fi
    else
        print_warning "Project network 'dotnetskills-network' does not exist"
    fi
    
    echo ""
}

# Function to provide quick actions
show_quick_actions() {
    print_header "Quick Actions"
    
    echo "Common commands:"
    echo "  Start services:    docker-compose up -d"
    echo "  Stop services:     docker-compose down"
    echo "  View logs:         ./Docker/scripts/logs.sh [service]"
    echo "  Restart:           ./Docker/scripts/restart.sh"
    echo "  Cleanup:           ./Docker/scripts/cleanup.sh"
    echo ""
    echo "Service URLs:"
    echo "  API:               http://localhost:8080"
    echo "  Swagger UI:        http://localhost:8080/swagger"
    echo "  Database:          localhost:1433 (sa/DevPassword123)"
    echo ""
}

# Main execution
main() {
    # Navigate to project root (assumes script is in Docker/scripts/)
    cd "$(dirname "$0")/../.."
    
    local show_services=true
    local show_health=true
    local show_resources=true
    local show_connectivity=true
    local json_output=false
    
    # Parse arguments
    while [[ $# -gt 0 ]]; do
        case $1 in
            --services)
                show_services=true
                show_health=false
                show_resources=false
                show_connectivity=false
                shift
                ;;
            --health)
                show_services=false
                show_health=true
                show_resources=false
                show_connectivity=false
                shift
                ;;
            --resources)
                show_services=false
                show_health=false
                show_resources=true
                show_connectivity=false
                shift
                ;;
            --connectivity)
                show_services=false
                show_health=false
                show_resources=false
                show_connectivity=true
                shift
                ;;
            --json)
                json_output=true
                shift
                ;;
            --help|-h)
                show_help
                exit 0
                ;;
            *)
                print_error "Unknown option: $1"
                show_help
                exit 1
                ;;
        esac
    done
    
    # TODO: Implement JSON output if requested
    if [[ $json_output == true ]]; then
        print_error "JSON output not yet implemented"
        exit 1
    fi
    
    # Show status sections
    echo -e "${BOLD}${BLUE}DotNetSkills Environment Status Report${NC}"
    echo -e "${BLUE}Generated: $(date)${NC}"
    echo ""
    
    show_docker_info
    
    if [[ $show_services == true ]]; then
        show_service_status
        show_environment
        show_network_info
    fi
    
    if [[ $show_health == true ]]; then
        check_service_health
    fi
    
    if [[ $show_resources == true ]]; then
        show_resource_usage
    fi
    
    if [[ $show_connectivity == true ]]; then
        test_connectivity
    fi
    
    show_quick_actions
}

# Execute main function with all arguments
main "$@"