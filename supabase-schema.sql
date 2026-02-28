-- Trickerion Board Game - Supabase Database Schema
-- Supabase SQL Editor에서 실행하세요.

-- 게임 세이브 테이블
CREATE TABLE game_saves (
  id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
  name TEXT NOT NULL,
  game_state JSONB NOT NULL,
  player_names TEXT[] NOT NULL DEFAULT '{}',
  round INTEGER NOT NULL DEFAULT 1,
  phase TEXT NOT NULL DEFAULT 'SETUP',
  num_players INTEGER NOT NULL DEFAULT 2,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- 업데이트 시 updated_at 자동 갱신
CREATE OR REPLACE FUNCTION update_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER set_updated_at
  BEFORE UPDATE ON game_saves
  FOR EACH ROW
  EXECUTE FUNCTION update_updated_at();

-- RLS 활성화 (향후 인증 추가 대비)
ALTER TABLE game_saves ENABLE ROW LEVEL SECURITY;

-- 현재는 모든 접근 허용 (익명 사용)
CREATE POLICY "Allow anonymous access" ON game_saves
  FOR ALL USING (true) WITH CHECK (true);

-- 인덱스
CREATE INDEX idx_game_saves_updated ON game_saves(updated_at DESC);
