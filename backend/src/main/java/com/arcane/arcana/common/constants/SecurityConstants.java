package com.arcane.arcana.common.constants;

/**
 * 보안 관련 상수를 정의
 */
public final class SecurityConstants {

    private SecurityConstants() {
        // 인스턴스 생성 방지
    }

    public static final String JWT_HEADER = "Authorization";
    public static final String REFRESH_TOKEN_HEADER = "Refresh-Token";
}
