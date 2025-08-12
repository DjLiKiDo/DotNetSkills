#!/bin/bash

# ==============================================================================
# DotNetSkills Local Environment Cleanup Script
# ==============================================================================
# This script provides comprehensive cleanup options for the local Docker environment
# Usage: ./cleanup.sh [option]
# Options:
#   --containers-only   Stop and remove containers only (keep volumes and images)
#   --full             Complete cleanup (containers, volumes, images, and networks)
#   --volumes-only     Remove volumes only (dangerous - data loss)
#   --images-only      Remove project-specific images only
#   --help             Show this help message

set -euo pipefail

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Project-specific identifiers
PROJECT_NAME="dotnetskills"
COMPOSE_FILE="docker-compose.yml"

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

# Function to check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running or not accessible"
        exit 1
    fi
}

# Function to show help
show_help() {
    cat << EOF
DotNetSkills Local Environment Cleanup Script

Usage: ./cleanup.sh [OPTION]

OPTIONS:
    --containers-only   Stop and remove containers only (keeps volumes and images)
    --full              Complete cleanup (containers, volumes, images, and networks)
    --volumes-only      Remove data volumes only (⚠️  DATA LOSS WARNING)
    --images-only       Remove project-specific Docker images only
    --help              Show this help message

EXAMPLES:
    ./cleanup.sh                    # Interactive mode - prompts for cleanup type
    ./cleanup.sh --containers-only  # Safe cleanup - stops containers but keeps data
    ./cleanup.sh --full            # Complete cleanup - removes everything

⚠️  WARNING: --volumes-only and --full options will permanently delete all data!

For more information, see: docs/SECURITY-DEPLOYMENT.md
EOF
}

# Function to stop and remove containers
cleanup_containers() {
    print_status "Stopping and removing Docker containers..."
    
    if [ -f "$COMPOSE_FILE" ]; then
        # Stop all services defined in docker-compose
        docker-compose down --remove-orphans
        print_success "Docker Compose services stopped and removed"
    else
        print_warning "docker-compose.yml not found, cleaning up individual containers"
    fi
    
    # Remove any remaining containers with project name
    CONTAINERS=$(docker ps -aq --filter "name=${PROJECT_NAME}" 2>/dev/null || true)
    if [ ! -z "$CONTAINERS" ]; then
        docker rm -f $CONTAINERS
        print_success "Remaining project containers removed"
    else
        print_status "No additional containers to remove"
    fi
}

# Function to remove volumes
cleanup_volumes() {
    print_warning "⚠️  This will permanently delete all database data!"
    read -p "Are you absolutely sure? Type 'YES' to continue: " confirmation
    
    if [ "$confirmation" != "YES" ]; then
        print_status "Volume cleanup cancelled"
        return
    fi
    
    print_status "Removing Docker volumes..."
    
    # Remove project-specific volumes
    VOLUMES=$(docker volume ls -q --filter "name=${PROJECT_NAME}" 2>/dev/null || true)
    if [ ! -z "$VOLUMES" ]; then
        docker volume rm $VOLUMES
        print_success "Project volumes removed"
    else
        print_status "No project volumes found"
    fi
}

# Function to remove images
cleanup_images() {
    print_status "Removing Docker images..."
    
    # Remove project-specific images
    IMAGES=$(docker images -q "*${PROJECT_NAME}*" 2>/dev/null || true)
    if [ ! -z "$IMAGES" ]; then
        docker rmi $IMAGES
        print_success "Project images removed"
    else
        print_status "No project images found"
    fi
    
    # Remove dangling images
    DANGLING=$(docker images -f "dangling=true" -q 2>/dev/null || true)
    if [ ! -z "$DANGLING" ]; then
        docker rmi $DANGLING
        print_success "Dangling images removed"
    fi
}

# Function to remove networks
cleanup_networks() {
    print_status "Removing Docker networks..."
    
    # Remove project-specific networks
    NETWORKS=$(docker network ls -q --filter "name=${PROJECT_NAME}" 2>/dev/null || true)
    if [ ! -z "$NETWORKS" ]; then
        docker network rm $NETWORKS
        print_success "Project networks removed"
    else
        print_status "No project networks found"
    fi
}

# Function for interactive cleanup
interactive_cleanup() {
    echo "=== DotNetSkills Environment Cleanup ==="
    echo ""
    echo "Select cleanup type:"
    echo "1) Containers only (safe - keeps data)"
    echo "2) Full cleanup (⚠️  removes all data)"
    echo "3) Volumes only (⚠️  data loss)"
    echo "4) Images only"
    echo "5) Cancel"
    echo ""
    read -p "Enter your choice (1-5): " choice
    
    case $choice in
        1)
            cleanup_containers
            ;;
        2)
            cleanup_containers
            cleanup_volumes
            cleanup_images
            cleanup_networks
            print_success "Full cleanup completed"
            ;;
        3)
            cleanup_volumes
            ;;
        4)
            cleanup_images
            ;;
        5)
            print_status "Cleanup cancelled"
            exit 0
            ;;
        *)
            print_error "Invalid choice"
            exit 1
            ;;
    esac
}

# Main execution
main() {
    check_docker
    
    # Navigate to project root (assumes script is in Docker/scripts/)
    cd "$(dirname "$0")/../.."
    
    # Parse command line arguments
    case "${1:-}" in
        --containers-only)
            cleanup_containers
            ;;
        --full)
            cleanup_containers
            cleanup_volumes
            cleanup_images
            cleanup_networks
            print_success "Full cleanup completed"
            ;;
        --volumes-only)
            cleanup_volumes
            ;;
        --images-only)
            cleanup_images
            ;;
        --help|-h)
            show_help
            ;;
        "")
            interactive_cleanup
            ;;
        *)
            print_error "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
    
    # Final status check
    print_status "Cleanup completed successfully"
    print_status "Use './Docker/scripts/restart.sh' to start fresh environment"
}

# Execute main function with all arguments
main "$@"