# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0.2"
    }
  }

  required_version = ">= 1.1.0"
  
  cloud {
    organization = "erst"
    workspaces {
      name = "kvitta"
    }
  }
}

provider "azurerm" {
  features {}
}

variable "resourceGroup" {
  type    = string
  default = "kvitta"
}

variable "location" {
  default = "swedencentral"
}

resource "azurerm_resource_group" "resourceGroup" {
  name     = var.resourceGroup
  location = var.location
}

resource "azurerm_container_registry" "container-registry" {
  location            = var.location
  name                = "kvitta-registry"
  resource_group_name = var.resourceGroup
  sku                 = "Basic"
}