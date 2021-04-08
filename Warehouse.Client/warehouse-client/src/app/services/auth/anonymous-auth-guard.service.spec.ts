import { TestBed } from '@angular/core/testing';

import { AnonymousAuthGuard } from './anonymous-auth-guard.service';

describe('AnonymousAuthGuardService', () => {
  let service: AnonymousAuthGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AnonymousAuthGuard);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
