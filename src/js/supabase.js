// Trickerion - Supabase Client & Game Save/Load API
import { SUPABASE_URL, SUPABASE_ANON_KEY } from './config.js';

let supabaseClient = null;

export function isConfigured() {
  return !!(SUPABASE_URL && SUPABASE_ANON_KEY);
}

export async function initSupabase() {
  if (!isConfigured()) return null;
  if (supabaseClient) return supabaseClient;
  try {
    const { createClient } = window.supabase;
    supabaseClient = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);
    return supabaseClient;
  } catch (e) {
    console.warn('Supabase 초기화 실패:', e);
    return null;
  }
}

export async function saveGame(name, engineState) {
  const client = await initSupabase();
  if (!client) return { success: false, message: 'Supabase가 설정되지 않았습니다.' };
  try {
    const playerNames = engineState.players.map(p => p.name);
    const { data, error } = await client.from('game_saves').insert({
      name,
      game_state: engineState,
      player_names: playerNames,
      round: engineState.round,
      phase: engineState.phase,
      num_players: engineState.numPlayers
    }).select().single();
    if (error) throw error;
    return { success: true, data };
  } catch (e) {
    console.error('저장 실패:', e);
    return { success: false, message: e.message };
  }
}

export async function updateSave(saveId, engineState) {
  const client = await initSupabase();
  if (!client) return { success: false, message: 'Supabase가 설정되지 않았습니다.' };
  try {
    const { data, error } = await client.from('game_saves').update({
      game_state: engineState,
      round: engineState.round,
      phase: engineState.phase,
      player_names: engineState.players.map(p => p.name)
    }).eq('id', saveId).select().single();
    if (error) throw error;
    return { success: true, data };
  } catch (e) {
    return { success: false, message: e.message };
  }
}

export async function listSaves() {
  const client = await initSupabase();
  if (!client) return { success: false, message: 'Supabase가 설정되지 않았습니다.', data: [] };
  try {
    const { data, error } = await client.from('game_saves')
      .select('id, name, player_names, round, phase, num_players, created_at, updated_at')
      .order('updated_at', { ascending: false })
      .limit(20);
    if (error) throw error;
    return { success: true, data: data || [] };
  } catch (e) {
    return { success: false, message: e.message, data: [] };
  }
}

export async function loadGame(saveId) {
  const client = await initSupabase();
  if (!client) return { success: false, message: 'Supabase가 설정되지 않았습니다.' };
  try {
    const { data, error } = await client.from('game_saves')
      .select('*').eq('id', saveId).single();
    if (error) throw error;
    return { success: true, data };
  } catch (e) {
    return { success: false, message: e.message };
  }
}

export async function deleteSave(saveId) {
  const client = await initSupabase();
  if (!client) return { success: false, message: 'Supabase가 설정되지 않았습니다.' };
  try {
    const { error } = await client.from('game_saves').delete().eq('id', saveId);
    if (error) throw error;
    return { success: true };
  } catch (e) {
    return { success: false, message: e.message };
  }
}
