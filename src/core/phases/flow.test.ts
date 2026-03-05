// Full game flow test: SETUP → ADVERTISE → ASSIGNMENT → PLACEMENT
// Reproduce: "극장 2번 선택했는데 반영안돼"

import { describe, it, expect } from 'vitest';
import { createGame } from '../state/init';
import { dispatch } from '../dispatch';
import { executeSetup } from './setup';
import { checkAdvertiseComplete } from './advertise';
import { advanceToPlacement } from './assignment';
import type { PlayerId, GameState, PlayerConfig, AssignmentCardId, Location } from '../types';

const CONFIGS: readonly PlayerConfig[] = [
  { name: 'Alice', magicianId: 'MECHANIKER', isHuman: true,
    startingSpecialist: 'ENGINEER', startingComponents: ['METAL', 'METAL'] },
  { name: 'Bob', magicianId: 'OPTICIAN', isHuman: false,
    startingSpecialist: 'MANAGER', startingComponents: ['FABRIC', 'FABRIC'] },
];

function fullSetupToAssignment(): GameState {
  let s = createGame(CONFIGS, { seed: 42 });
  // SETUP → ADVERTISE
  s = executeSetup(s);
  expect(s.phase).toBe('ADVERTISE');

  // Both skip advertise
  let r = dispatch(s, { type: 'SKIP_ADVERTISE', playerId: 0 as PlayerId });
  s = r.state;
  r = dispatch(s, { type: 'SKIP_ADVERTISE', playerId: 1 as PlayerId });
  s = r.state;

  // finishAdvertise
  s = checkAdvertiseComplete(s);
  expect(s.phase).toBe('ASSIGNMENT');
  expect(s.assignmentPhase).not.toBeNull();

  return s;
}

describe('Full flow: SETUP → ASSIGNMENT → PLACEMENT', () => {
  it('assignment phase is properly initialized after advertise', () => {
    const s = fullSetupToAssignment();
    expect(s.assignmentPhase?.playersSubmitted).toEqual([]);
    expect(s.assignmentPhase?.currentAssigner).toBe(0);
  });

  it('player has 6 assignment cards', () => {
    const s = fullSetupToAssignment();
    const p0 = s.players[0];
    expect(p0.assignmentCards.length).toBe(6);

    // Check locations
    const locs = p0.assignmentCards.map(c => c.location);
    expect(locs.filter(l => l === 'THEATER').length).toBe(2);
    expect(locs.filter(l => l === 'DOWNTOWN').length).toBe(1);
    expect(locs.filter(l => l === 'MARKET_ROW').length).toBe(1);
    expect(locs.filter(l => l === 'WORKSHOP').length).toBe(1);
    expect(locs.filter(l => l === 'DARK_ALLEY').length).toBe(1);
  });

  it('can assign characters to BOTH theater cards', () => {
    let s = fullSetupToAssignment();
    const p0 = s.players[0];

    // Find theater cards
    const theaterCards = p0.assignmentCards.filter(c => c.location === 'THEATER');
    expect(theaterCards.length).toBe(2);

    // Assign character 0 (MAGICIAN) to theater card #1
    let r = dispatch(s, {
      type: 'PLACE_ASSIGNMENT_CARD',
      playerId: 0 as PlayerId,
      cardId: theaterCards[0].id,
      characterIndices: [0],
    });
    expect(r.result.ok).toBe(true);
    s = r.state;
    expect(s.players[0].currentPlacements.length).toBe(1);

    // Assign character 1 (APPRENTICE) to theater card #2
    r = dispatch(s, {
      type: 'PLACE_ASSIGNMENT_CARD',
      playerId: 0 as PlayerId,
      cardId: theaterCards[1].id,
      characterIndices: [1],
    });
    expect(r.result.ok).toBe(true);
    s = r.state;
    expect(s.players[0].currentPlacements.length).toBe(2);

    // Both placements are for theater
    const placements = s.players[0].currentPlacements;
    for (const pl of placements) {
      const card = s.players[0].assignmentCards.find(c => c.id === pl.cardId);
      expect(card?.location).toBe('THEATER');
    }
  });

  it('FINISH_ADVERTISE action properly initializes assignment phase', () => {
    let s = createGame(CONFIGS, { seed: 42 });
    s = executeSetup(s);

    // Both skip
    let r = dispatch(s, { type: 'SKIP_ADVERTISE', playerId: 0 as PlayerId });
    s = r.state;
    r = dispatch(s, { type: 'SKIP_ADVERTISE', playerId: 1 as PlayerId });
    s = r.state;

    // Use FINISH_ADVERTISE action (this is what the UI might use)
    r = dispatch(s, { type: 'FINISH_ADVERTISE' });
    expect(r.result.ok).toBe(true);
    s = r.state;

    expect(s.phase).toBe('ASSIGNMENT');
    expect(s.assignmentPhase).not.toBeNull();
    expect(s.assignmentPhase?.playersSubmitted).toEqual([]);
  });

  it('full flow: assign → submit → reveal → placement with theater', () => {
    let s = fullSetupToAssignment();
    const p0 = s.players[0];
    const p1 = s.players[1];

    // Alice: assign magician to theater #1, apprentice to theater #2
    const aliceTheater = p0.assignmentCards.filter(c => c.location === 'THEATER');
    let r = dispatch(s, {
      type: 'PLACE_ASSIGNMENT_CARD', playerId: 0 as PlayerId,
      cardId: aliceTheater[0].id, characterIndices: [0],
    });
    s = r.state;
    r = dispatch(s, {
      type: 'PLACE_ASSIGNMENT_CARD', playerId: 0 as PlayerId,
      cardId: aliceTheater[1].id, characterIndices: [1],
    });
    s = r.state;

    // Alice submit
    r = dispatch(s, { type: 'SUBMIT_ASSIGNMENT', playerId: 0 as PlayerId });
    expect(r.result.ok).toBe(true);
    s = r.state;

    // Bob: assign magician to downtown
    const bobDowntown = s.players[1].assignmentCards.find(c => c.location === 'DOWNTOWN')!;
    r = dispatch(s, {
      type: 'PLACE_ASSIGNMENT_CARD', playerId: 1 as PlayerId,
      cardId: bobDowntown.id, characterIndices: [0],
    });
    s = r.state;
    r = dispatch(s, { type: 'SUBMIT_ASSIGNMENT', playerId: 1 as PlayerId });
    s = r.state;

    // Reveal
    r = dispatch(s, { type: 'REVEAL_ASSIGNMENTS' });
    expect(r.result.ok).toBe(true);
    s = r.state;
    expect(s.phase).toBe('ASSIGNMENT_REVEAL');

    // Check Alice's characters are assigned to THEATER
    const alice = s.players[0];
    expect(alice.characters[0].assigned).toBe(true);
    expect(alice.characters[0].location).toBe('THEATER');
    expect(alice.characters[1].assigned).toBe(true);
    expect(alice.characters[1].location).toBe('THEATER');

    // Advance to placement
    s = advanceToPlacement(s);
    expect(s.phase).toBe('PLACEMENT');
    expect(s.turnQueue.length).toBeGreaterThan(0);

    // Alice should have 2 entries in turn queue (2 theater chars)
    const aliceTurns = s.turnQueue.filter(t => t.playerId === 0);
    expect(aliceTurns.length).toBe(2);
  });
});
