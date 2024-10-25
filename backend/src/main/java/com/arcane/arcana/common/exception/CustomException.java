package com.arcane.arcana.common.exception;

import org.springframework.http.HttpStatus;

/**
 * 사용자 정의 예외 클래스
 */
public class CustomException extends RuntimeException {
    private final HttpStatus status;

    public CustomException(String message, HttpStatus status) {
        super(message);
        this.status = status;
    }

    public HttpStatus getStatus() {
        return status;
    }
}
