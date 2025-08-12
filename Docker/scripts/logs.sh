#!/bin/bash

# ==============================================================================
# DotNetSkills Docker Logs Viewer Script
# ==============================================================================
# This script provides convenient access to Docker container logs with filtering
# and follow options for debugging and monitoring.
# Usage: ./logs.sh [service] [options]

set -euo pipefail

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to show help
show_help() {
    cat << EOF
DotNetSkills Docker Logs Viewer

Usage: ./logs.sh [SERVICE] [OPTIONS]

SERVICES:
    api       Show API container logs
    db        Show database container logs  
    redis     Show Redis cache logs
    all       Show all service logs (default)

OPTIONS:
    -f, --follow     Follow log output (like 'tail -f')
    -t, --tail N     Show last N lines (default: 100)
    --since TIME     Show logs since timestamp (e.g., '2024-01-01', '1h', '30m')
    --help           Show this help message

EXAMPLES:
    ./logs.sh                    # Show all logs (last 100 lines)
    ./logs.sh api --follow       # Follow API logs in real-time
    ./logs.sh db --tail 50       # Show last 50 database log lines
    ./logs.sh all --since 1h     # Show all logs from last hour
    ./logs.sh api --since '2024-08-11 10:00:00'  # Logs since specific time

FILTERING LOGS:
    You can pipe the output to grep for filtering:
    ./logs.sh api | grep ERROR   # Show only error lines
    ./logs.sh api | grep -i sql  # Show SQL-related logs (case insensitive)
EOF
}

# Function to check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running or not accessible"
        exit 1
    fi
}

# Function to get running services
get_running_services() {
    docker-compose ps --services --filter "status=running"
}

# Function to show logs for a specific service
show_service_logs() {
    local service=$1
    local options="${2:-}"
    
    # Check if service exists and is running
    if ! get_running_services | grep -q "^${service}$"; then
        print_error "Service '$service' is not running"
        print_status "Available running services:"
        get_running_services | sed 's/^/  - /'
        exit 1
    fi
    
    print_status "Showing logs for service: $service"
    echo "Press Ctrl+C to stop following logs"
    echo "----------------------------------------"
    
    docker-compose logs $options "$service"
}

# Function to show all logs
show_all_logs() {
    local options="${1:-}"
    
    print_status "Showing logs for all services"
    echo "Press Ctrl+C to stop following logs"
    echo "----------------------------------------"
    
    docker-compose logs $options
}

# Main execution
main() {
    check_docker
    
    # Navigate to project root (assumes script is in Docker/scripts/)
    cd "$(dirname "$0")/../.."
    
    # Check if docker-compose.yml exists
    if [ ! -f "docker-compose.yml" ]; then
        print_error "docker-compose.yml not found in current directory"
        exit 1
    fi
    
    local service=""
    local options=""
    local tail_lines="100"
    local follow=false
    local since=""
    
    # Parse arguments
    while [[ $# -gt 0 ]]; do
        case $1 in
            api|db|redis)
                service=$1
                shift
                ;;
            all|"")
                service="all"
                shift
                ;;
            -f|--follow)
                follow=true
                shift
                ;;
            -t|--tail)
                if [[ -n ${2:-} ]] && [[ $2 =~ ^[0-9]+$ ]]; then
                    tail_lines=$2
                    shift 2
                else
                    print_error "Invalid tail value. Please provide a number."
                    exit 1
                fi
                ;;
            --since)
                if [[ -n ${2:-} ]]; then
                    since=$2
                    shift 2
                else
                    print_error "Invalid since value. Please provide a timestamp."
                    exit 1
                fi
                ;;
            --help|-h)
                show_help
                exit 0
                ;;
            *)
                if [[ -z $service ]]; then
                    service=$1
                else
                    print_error "Unknown option: $1"
                    show_help
                    exit 1
                fi
                shift
                ;;
        esac
    done
    
    # Default to all if no service specified
    if [[ -z $service ]]; then
        service="all"
    fi
    
    # Build options string
    if [[ $follow == true ]]; then
        options="$options --follow"
    fi
    
    options="$options --tail $tail_lines"
    
    if [[ -n $since ]]; then
        options="$options --since '$since'"
    fi
    
    # Show logs
    case $service in
        all)
            show_all_logs "$options"
            ;;
        api|db|redis)
            show_service_logs "$service" "$options"
            ;;
        *)
            print_error "Unknown service: $service"
            print_status "Available services: api, db, redis, all"
            exit 1
            ;;
    esac
}

# Execute main function with all arguments
main "$@"