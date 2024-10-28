package com.arcane.arcana.common.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;

/**
 * 공통 응답 포맷을 위한 클래스
 */
@Getter
@Setter
@AllArgsConstructor
public class ApiResponse {
    private String message;
    private Object data;
}
