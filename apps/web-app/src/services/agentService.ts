/**
 * Agent Service
 * API calls for agent management
 */

import apiClient, { getApiErrorMessage } from './apiClient';
import type { AgentDto, GetAgentsParams, GetAgentsResponse } from './types';

const BASE_PATH = '/agents';

export const agentService = {
  /**
   * Get list of agents
   */
  async getAgents(params?: GetAgentsParams): Promise<GetAgentsResponse> {
    try {
      const response = await apiClient.get<GetAgentsResponse>(BASE_PATH, { params });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch agents:', getApiErrorMessage(error));
      throw error;
    }
  },

  /**
   * Get agent by ID
   */
  async getAgentById(id: string): Promise<AgentDto> {
    try {
      const response = await apiClient.get<AgentDto>(`${BASE_PATH}/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch agent ${id}:`, getApiErrorMessage(error));
      throw error;
    }
  },

  /**
   * Create new agent
   */
  async createAgent(agent: Partial<AgentDto>): Promise<AgentDto> {
    try {
      const response = await apiClient.post<AgentDto>(BASE_PATH, agent);
      return response.data;
    } catch (error) {
      console.error('Failed to create agent:', getApiErrorMessage(error));
      throw error;
    }
  },

  /**
   * Update agent
   */
  async updateAgent(id: string, updates: Partial<AgentDto>): Promise<AgentDto> {
    try {
      const response = await apiClient.put<AgentDto>(`${BASE_PATH}/${id}`, updates);
      return response.data;
    } catch (error) {
      console.error(`Failed to update agent ${id}:`, getApiErrorMessage(error));
      throw error;
    }
  },

  /**
   * Delete agent
   */
  async deleteAgent(id: string): Promise<void> {
    try {
      await apiClient.delete(`${BASE_PATH}/${id}`);
    } catch (error) {
      console.error(`Failed to delete agent ${id}:`, getApiErrorMessage(error));
      throw error;
    }
  },
};

export default agentService;
