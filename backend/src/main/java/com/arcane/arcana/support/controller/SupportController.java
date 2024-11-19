package com.arcane.arcana.support.controller;

import com.arcane.arcana.support.service.SupportService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.parameters.RequestBody;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

@RestController
@RequestMapping("/support")
public class SupportController {

    private final SupportService supportService;

    public SupportController(SupportService supportService) {
        this.supportService = supportService;
    }

    @Operation(summary = "고객 지원 요청 제출", description = "고객 지원 요청을 제출하며, 필요시 스크린샷을 포함할 수 있습니다.")
    @PostMapping(consumes = "multipart/form-data")
    public ResponseEntity<String> handleSupportRequest(
        @Parameter(description = "문의 유형") @RequestParam("category") String category,
        @Parameter(description = "고객 이메일") @RequestParam("email") String email,
        @Parameter(description = "문의 제목") @RequestParam("title") String title,
        @Parameter(description = "문의 설명") @RequestParam("description") String description,
        @RequestPart(value = "screenshots", required = false)
        @Parameter(description = "문의와 함께 첨부할 스크린샷들", content = @Content(mediaType = "application/octet-stream")) MultipartFile[] screenshots) {

        try {
            supportService.processSupportRequest(category, email, title, description, screenshots);
            return ResponseEntity.ok("문의가 성공적으로 접수되었습니다.");
        } catch (Exception e) {
            return ResponseEntity.status(500).body("문의 접수 중 오류가 발생했습니다.");
        }
    }
}
