package com.arcane.arcana.common.service;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import software.amazon.awssdk.core.ResponseBytes;
import software.amazon.awssdk.core.sync.RequestBody;
import software.amazon.awssdk.services.s3.S3Client;
import software.amazon.awssdk.services.s3.model.GetObjectResponse;
import software.amazon.awssdk.services.s3.model.PutObjectRequest;
import software.amazon.awssdk.services.s3.model.DeleteObjectRequest;
import software.amazon.awssdk.services.s3.model.GetObjectRequest;
import software.amazon.awssdk.services.s3.presigner.S3Presigner;
import software.amazon.awssdk.services.s3.presigner.model.GetObjectPresignRequest;
import software.amazon.awssdk.services.s3.presigner.model.PresignedGetObjectRequest;
import software.amazon.awssdk.services.s3.presigner.model.PutObjectPresignRequest;
import software.amazon.awssdk.services.s3.presigner.model.PresignedPutObjectRequest;

import java.net.URL;
import java.time.Duration;

/**
 * S3 관련 기능을 제공
 */
@Service
public class S3Service {

    private final S3Client s3Client;
    private final S3Presigner s3Presigner;

    @Value("${cloud.aws.s3.bucket}")
    private String bucketName;

    public S3Service(S3Client s3Client, S3Presigner s3Presigner) {
        this.s3Client = s3Client;
        this.s3Presigner = s3Presigner;
    }

    /**
     * 파일을 업로드
     */
    public String uploadFile(String key, byte[] data) {
        s3Client.putObject(PutObjectRequest.builder().bucket(bucketName).key(key).build(),
            RequestBody.fromBytes(data));
        return key;
    }

    /**
     * 파일을 삭제
     */
    public void deleteFile(String key) {
        s3Client.deleteObject(DeleteObjectRequest.builder().bucket(bucketName).key(key).build());
    }

    /**
     * 파일을 다운
     */
    public byte[] downloadFile(String key) {
        GetObjectRequest getObjectRequest = GetObjectRequest.builder()
            .bucket(bucketName)
            .key(key)
            .build();

        ResponseBytes<GetObjectResponse> objectBytes = s3Client.getObjectAsBytes(getObjectRequest);

        return objectBytes.asByteArray();
    }

    /**
     * 프리사인드 GET URL을 생성
     */
    public URL generatePresignedUrl(String key, long durationInMinutes) {
        PresignedGetObjectRequest presignedRequest = s3Presigner.presignGetObject(
            GetObjectPresignRequest.builder()
                .signatureDuration(Duration.ofMinutes(durationInMinutes))
                .getObjectRequest(GetObjectRequest.builder()
                    .bucket(bucketName)
                    .key(key)
                    .build())
                .build());
        return presignedRequest.url();
    }

    /**
     * 프리사인드 PUT URL을 생성
     */
    public URL generateUploadPresignedUrl(String key) {
        PresignedPutObjectRequest presignedRequest = s3Presigner.presignPutObject(
            PutObjectPresignRequest.builder()
                .signatureDuration(Duration.ofMinutes(10))
                .putObjectRequest(PutObjectRequest.builder()
                    .bucket(bucketName)
                    .key(key)
                    .build())
                .build());
        return presignedRequest.url();
    }
}
