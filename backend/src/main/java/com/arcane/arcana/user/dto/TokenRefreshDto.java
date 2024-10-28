package com.arcane.arcana.user.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 토큰 갱신 요청을 위한 DTO
 */
@Getter
@Setter
@NoArgsConstructor
public class TokenRefreshDto {

    private String refreshToken;
}
