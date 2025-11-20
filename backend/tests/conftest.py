"""pytest configuration and fixtures"""

import pytest


@pytest.fixture
def sample_workflow_data():
    """Sample workflow data for testing"""
    return {
        "name": "Test Workflow",
        "description": "A test workflow for unit testing",
        "steps": [
            {
                "id": "step1",
                "type": "prompt",
                "config": {"template": "Hello {{name}}"},
            }
        ],
    }
